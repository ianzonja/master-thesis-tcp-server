using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace TCPserver
{
    class TCPserver
    {
        TcpListener listener;
        TcpClient klijent;
        NetworkStream stream;
        //string testPoruka = "Test poruka poslana";
        //List<string> podaci;
        byte[] writeBuffer;
        byte[] readBuffer;
        string primljenaPoruka;

        public void PokreniListener()
        {
            listener = new TcpListener(IPAddress.Loopback, 9950);
            Thread dretvaZaListen = new Thread(new ParameterizedThreadStart(OsluskujPort));
            dretvaZaListen.Start(listener);
        }
        void OsluskujPort(object listen)
        {
            listener = (TcpListener)listen;
            listener.Start();
            while (true)
            {
                klijent = listener.AcceptTcpClient();
                Console.WriteLine("Spojen");
                ObradiKlijenta(klijent);
            }
        }

        void ObradiKlijenta(TcpClient klijent)
        {
            writeBuffer = new byte[1900];
            readBuffer = new byte[1900];
            string responseJson = "";
            string klijentPodaciString = "";
            stream = klijent.GetStream();
            if (stream.CanRead)
            {
                stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                primljenaPoruka = Encoding.ASCII.GetString(readBuffer);
                Console.WriteLine("primljena:  " + primljenaPoruka);
                //Enkriptiraj poruku()
                ObradaPoruke obrada = new ObradaPoruke(readBuffer);
                responseJson = obrada.PrepoznavanjePoruke();
                stream.Flush();
            }
            if (stream.CanWrite)
            {
                if (responseJson != "")
                {
                    writeBuffer = Encoding.ASCII.GetBytes(responseJson);
                    stream.Write(writeBuffer, 0, writeBuffer.Length);
                    stream.Flush();
                }
                else
                {
                    writeBuffer = Encoding.ASCII.GetBytes("Netocni podaci");
                    stream.Write(writeBuffer, 0, writeBuffer.Length);
                    stream.Flush();
                }
            }
        }
    }
}
