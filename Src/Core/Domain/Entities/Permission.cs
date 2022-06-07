using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Class Permission
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Permission id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Determines if user can read
        /// </summary>
        public bool Read { get; set; }
        /// <summary>
        /// Determines if user can write
        /// </summary>
        public bool Write { get; set; }
        /// <summary>
        /// Determines if user can delete
        /// </summary>
        public bool Delete { get; set; }
        /// <summary>
        /// Group Shares
        /// </summary>
        public ICollection<GroupShare> GroupShares { get; set; }
        /// <summary>
        /// User Shares
        /// </summary>
        public ICollection<UserShare> UserShares { get; set; }
    }
}
