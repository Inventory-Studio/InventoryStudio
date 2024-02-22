using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace ISLibrary
{
    public abstract class BaseClass
    {
        public bool IsLoaded { get; protected set; }
        public string PrimaryKey { get; protected set; }
        public string ParentKey { get;  set; }
        public string ParentObject { get;  set; }
        protected Dictionary<string, object> OriginalValues { get; set; }
        protected Dictionary<string, object> CurrentValues { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        // if the data is null, system will record all the attributes changed, if it have value, system will trace the attributes in it
        protected virtual List<string> TraceAttributes { get; set; }

        public enum enumActionType
        {
            [Description("Create")]
            Create,
            [Description("Update")]
            Update,
            [Description("Delete")]
            Delete
        }

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

        public void LogAuditData(enumActionType action)
        {
            MapValues(CurrentValues);
            RecordChange(action);
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

        protected void RecordChange(enumActionType action)
        {
            var changedProperties = new Dictionary<string, object>();
         
            if(action== enumActionType.Delete)
            {
                 changedProperties = OriginalValues;
            }
            else
            {
                 changedProperties = GetChangedProperties();
            }
            
            if (changedProperties.Count > 0)
            {                             
                // Serialize auditData to JSON and store it
                string auditDataJson = JsonSerializer.Serialize(changedProperties);

                var auditData = new AuditData();
                auditData.ObjectID = PrimaryKey;
                auditData.ObjectName = this.GetType().Name;
                auditData.ParentKey = ParentKey;
                auditData.ParentObject = ParentObject;
                auditData.ChangedValue = auditDataJson;
                auditData.CreatedBy = UpdatedBy ?? CreatedBy ;
                auditData.Type = action.ToString();
                
                auditData.Create();


            }
            
        }

        private Dictionary<string, object> GetChangedProperties()

        {
            var changes = new Dictionary<string, object>();


            foreach (var kvp in CurrentValues)
            {
                // Skip comparison for ParentKey and ParentObject
                if (kvp.Key == "ParentKey" || kvp.Key == "ParentObject")
                {
                    continue;
                }

                OriginalValues.TryGetValue(kvp.Key, out var originalValue);

                // Skip if both originalValue and kvp.Value are null or empty
                if (String.IsNullOrEmpty(originalValue?.ToString()) && String.IsNullOrEmpty(kvp.Value?.ToString()))
                {
                    continue;
                }

                if (!Equals(originalValue, kvp.Value))
                {                   
                    // Check if originalValue has a specific value (not null and not empty string)
                    if (!String.IsNullOrEmpty(originalValue?.ToString()))
                    {
                        changes[kvp.Key] = new Dictionary<string, object>
                        {
                            { "OriginalValue", originalValue },
                            { "CurrentValue", kvp.Value }
                        };
                    }
                    else
                    {
                        changes[kvp.Key] = kvp.Value;
                    }
                    
                }       
            }
            return changes;
        }

        protected void MapValues(Dictionary<string, object> values)
        {           

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            bool primaryKeyRecorded = false;

            foreach (var property in properties)
            {
                // get PrimaryKey
                if (!primaryKeyRecorded && property.Name.ToLowerInvariant().Contains("id"))
                {
                    PrimaryKey = Convert.ToString(property.GetValue(this));
                    primaryKeyRecorded = true;
                }

                if (property.CanRead && ((TraceAttributes.Count > 0 && TraceAttributes.Contains(property.Name)) || TraceAttributes.Count == 0))
                {
                    // Check if the property type is an entity class (custom class) or a list of entity classes
                    bool isEntityClass = property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && !property.PropertyType.IsValueType && property.PropertyType != typeof(string);
                    bool isListOfEntityClass = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) && property.PropertyType.GetGenericArguments()[0].IsClass;

                    if (!(isEntityClass || isListOfEntityClass))
                    {
                        // If the property type is not an entity class or a list of entity classes, assign its value
                        // Translate: If the property type is neither an Object (entity class) nor a List of Objects (entity classes), then assign the value.
                       values[property.Name] = property.GetValue(this);
                                            
                    }
                }
            }

            // Directly assign UpdatedBy and CreatedBy if they exist in the dictionary
            if (values.ContainsKey("UpdatedBy"))
            {
                this.UpdatedBy = values["UpdatedBy"] as string; // Assumes the value is a string
            }

            if (values.ContainsKey("CreatedBy"))
            {
                this.CreatedBy = values["CreatedBy"] as string; // Assumes the value is a string
            }
        }
    }
}
