using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitialProject.Model.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }   
        
        public string Username { get; set; }

        public string CheckpointArrivalName { get; set; }
        public UserDTO()
        {
        }
        public UserDTO(int userId, string username, string checkpointArrivalName)
        {
            UserId = userId;
            Username = username;
            CheckpointArrivalName = checkpointArrivalName;
        }

    }
}
