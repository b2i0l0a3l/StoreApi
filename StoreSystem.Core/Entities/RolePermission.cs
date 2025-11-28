using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Core.Entities
{
    public class RolePermission : baseEntity
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PermissionId Must Greath than 0")]
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission? Permission { get; set; } 
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId Must Greath than 0")]
        public int RoleId { get; set; } 
        [ForeignKey("RoleId")]
        public Role? Roles { get; set; }

        
    }
}