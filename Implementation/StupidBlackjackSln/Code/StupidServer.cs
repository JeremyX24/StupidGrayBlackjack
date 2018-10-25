using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StupidBlackjackSln.Code {
    class StupidServer {

        public static const int ID_SIZE_IN_BYTES = 32;
        public static const int DEFAULT_PORT = 61537;
        public static const String DEFAULT_DOMAIN = "";
        public static const int MAX_COMMAND_LENGTH = 64;
        public static const String JOIN_SUCCESS = "1";
        public static const String FETCH_COMMAND = "f";
        public static const String HOST_NEW_GAME_COMMAND = "h";
        public static const String JOIN_GAME_BY_ID_COMMAND = "j";
        public static const String REMOVE_GAME_BY_ID_COMMAND = "r";

        private int port = DEFAULT_PORT;
        private ArrayList clients;
        private ArrayList streams;
        private TcpListener server;

        public StupidServer() {
            clients = new ArrayList();
            server = new TcpListener(this.port);
        }

        public StupidServer(int port) {
            this.port = port;
            clients = new ArrayList();
            server = new TcpListener(this.port);
        }

        private void Broadcast(String s) {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            lock (streams) {
                foreach (NetworkStream ns in streams) {
                    ns.Write(buffer, 0, buffer.Length());
                }
            }
        }

        /// <summary>
        /// Close all connections. Call this before terminating.
        /// </summary>
        public void Close() {
            lock (clients) {
                lock (streams) {
                    foreach (TcpClient c in clients) {
                        c.Close();
                    }
                    foreach (NetworkStream n in streams) {
                        n.Close();
                    }
                }
            }

            server.Close();
        }

        private void InterpretCommand(String cmd) {
            //TODO
        }

        private void LoopAccept() {
            while (true) {
                TcpClient c = server.AcceptTcpClient();
                
                lock (clients) {
                    clients.Add(c);
                }
                
                new Thread(LoopListen).Start(c);
            }
        }

        private void LoopListen(TcpClient c) {
            NetworkStream ns = c.GetStream();

            lock (streams) {
                streams.Add(ns);
            }

            byte[] buffer = new byte[MAX_COMMAND_LENGTH];
            while (true) {
                ns.Read(buffer, 0, MAX_COMMAND_LENGTH);
                this.InterpretCommand(Encoding.ASCII.GetString(buffer, 0, MAX_COMMAND_LENGTH));
            }
        }

        public void Start() {
            try {
                server.Start();
                //TODO add some status message
            } catch (Exception e) {
                //TODO
            }

            new Thread(LoopAccept).Start();
        }

        public void Stop() {
            //TODO
        }
    }
}
