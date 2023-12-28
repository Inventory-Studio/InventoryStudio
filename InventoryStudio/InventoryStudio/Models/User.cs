﻿using InventoryStudio.Data;
using ISLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryStudio.Models
{


    public class User : IdentityUser<int>
    {


        [MaxLength(10)]
        public string Status { get; set; } = "Active";

        [MaxLength(50)]
        public string UserType { get; set; } = "Normal";

        private IsUser mUser = null;
        public IsUser IsUser
        {
            get
            {
                if (mUser == null && Id>0)
                {

                    try
                    {
                        mUser = new IsUser(Id.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {

                    }
                }
                return mUser;
            }
        }

    }
}
