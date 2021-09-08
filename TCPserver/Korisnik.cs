using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Korisnik
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DatumRodjenja { get; set; }
        public int RazinaPristupa { get; set; } //moze biti 1,2,3 (ovisno je li admin,moderator,korisnik)
        public bool Aktivnost { get; set; }

        public void Dodaj(string username,string password,string email,string datum,int razina,bool aktivnost)
        {
            Username = username;
            Password = password;
            Email = email;
            DatumRodjenja = datum;
            RazinaPristupa = razina;
            Aktivnost = aktivnost;
        }

        public string GetUsername()
        {
            return this.Username;
        }

        public string GetPassword()
        {
            return this.Password;
        }

        public string GetEmail()
        {
            return this.Email;
        }

        public string GetDatumRodjenja()
        {
            return this.DatumRodjenja;
        }

        public int GetRazinaPristupa()
        {
            return this.RazinaPristupa;
        }

        public bool GetAktivnost()
        {
            return this.Aktivnost;
        }
    }
}
