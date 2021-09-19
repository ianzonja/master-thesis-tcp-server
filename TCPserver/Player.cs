using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    public class JwtPlayer : Player
    {
        public string Jwt { get; set; }

        public JwtPlayer SetPlayer(Player player)
        {
            this.Email = player.Email;
            this.Username = player.Username;
            this.Wins = player.Wins;
            this.Loses = player.Loses;
            this.Draws = player.Draws;
            this.PlayFabId = player.PlayFabId;
            this.Avatar = player.Avatar;
            return this;
        }
    }
    public class Player
    {
        public string Email { get; set; }

        public string Username { get; set; }
       
        public string Wins { get; set; }

        public string Loses { get; set; }

        public string Draws { get; set; }

        public string PlayFabId { get; set; }

        public string InGameStatus { get; set; }

        public string Avatar { get; set; }
    }
}
