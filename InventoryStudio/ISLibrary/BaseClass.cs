using System.Collections;
using System.Reflection;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace ISLibrary
{
    public abstract class BaseClass
    {
        public bool IsLoaded { get; protected set; }
        protected Dictionary<string, object> OriginalValues { get; set; }
        protected Dictionary<string, object> CurrentValues { get; set; }

        // if the data is null, system will record all the attributes changed, if it have value, system will trace the attributes in it
        protected virtual List<string> TraceAttributes { get; set; }

        public BaseClass()
        {
            OriginalValues = new Dictionary<string, object>();
            CurrentValues = new Dictionary<string, object>();
            TraceAttributes = new List<string>();
        }

        protected virtual void Load()
        {
            IsLoaded = true;
            MapValues(OriginalValues);
        }

        protected virtual void Load(SqlConnection objConn, SqlTransaction objTran)
        {
            IsLoaded = true;          
        }

        public virtual bool Create()
        {
            if (IsLoaded) throw new Exception("Create() cannot be performed because object is loaded from constructors");
            return true;
        }

        public virtual bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            if (IsLoaded) throw new Exception("Create() cannot be performed because object is loaded from constructors");
            //mConnection = objConn;
            //mTransaction = objTran;
            return true;
        }

        public virtual bool Update()
        {

            MapValues(CurrentValues);
            RecordChange("Update");
            if (!IsLoaded) throw new Exception("Update() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            if (!IsLoaded) throw new Exception("Update() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Copy()
        {
            IsLoaded = false;
            return true;
        }

        public virtual bool Copy(SqlConnection objConn, SqlTransaction objTran)
        {
            IsLoaded = false;
            return true;
        }

        public virtual bool Delete()
        {
            if (!IsLoaded) throw new Exception("Delete() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            if (!IsLoaded) throw new Exception("Delete() cannot be performed because object is not loaded from constructors");
            return true;
        }

        protected void RecordChange(string action)
        {
            var changes = GetChangedProperties();
            if (changes.Count > 0)
            {                
                // Serialize auditData to JSON and store it
                string auditDataJson = JsonSerializer.Serialize(changes);
               
                var auditData = new Dictionary<string, object>
                {
                    ["Action"] = action,
                    ["Changes"] = auditDataJson,
                    ["Timestamp"] = DateTime.UtcNow
                };
            }
            
        }

        private Dictionary<string, (object OriginalValue, object CurrentValue)> GetChangedProperties()

        {
            var changes = new Dictionary<string, (object OriginalValue, object CurrentValue)>();

            foreach (var kvp in CurrentValues)
            {
                if (OriginalValues.TryGetValue(kvp.Key, out var originalValue))
                {
                    if (!Equals(originalValue, kvp.Value))
                    {
                        changes[kvp.Key] = (originalValue, kvp.Value);
                    }
                }
                else
                {
                    changes[kvp.Key] = (null, kvp.Value);
                }
            }
            return changes;
        }

        protected void MapValues(Dictionary<string, object> values)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CanRead && ((TraceAttributes.Count > 0 && TraceAttributes.Contains(property.Name)) || TraceAttributes.Count == 0))
                {
                    values[property.Name] = property.GetValue(this);
                }
            }
        }
    }
}
