namespace GuiServer.Server.Database
{
    using Tables;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using View.ViewModel;

    public class DatabaseManager
    {
        private static DatabaseManager _databaseManager;

        public static DatabaseManager Instance
        {
            get
            {
                if (_databaseManager == null)
                {
                    _databaseManager = new DatabaseManager();
                }
                return _databaseManager;
            }
        }

        private SQLiteConnection connection;

        public DatabaseManager()
        {
            this.CreatDatabase();
            this.CreatTables();
        }

        public void CreatDatabase()
        {
            this.connection = new SQLiteConnection("RemoControl.sqlite3");
        }


        public void CreatTables()
        {
            this.connection.CreateTable<ClientTable>();
            this.connection.CreateTable<ChatTable>();
        }

        public ClientViewModel SelectClient(string UniqueHash)
        {
            ClientViewModel viewModel = null;

            ClientTable table = this.SelectClientTable(UniqueHash);
            if (table != null)
            {
                viewModel = new ClientViewModel(table);
            }

            return viewModel;
        }

        public ClientTable SelectClientTable(string UniqueHash)
        {
            TableQuery<ClientTable> query = connection.Table<ClientTable>().Where(cli => cli.UniqueHash.Equals(UniqueHash));
            ClientTable table = query.FirstOrDefault();
            return table;
        }


        public List<ClientViewModel> SelectAllClients()
        {
            TableQuery<ClientTable> query = connection.Table<ClientTable>();

            List<ClientViewModel> viewModels = new List<ClientViewModel>();
            foreach (ClientTable table in query)
            {
                viewModels.Add(new ClientViewModel(table));
            }

            return viewModels;
        }

        public void InsertClient(ClientViewModel clientViewModel)
        {
            ClientTable table = ClientTable.Create(clientViewModel);

            TableQuery<ClientTable> query = connection.Table<ClientTable>().Where(cli => cli.UniqueHash.Equals(table.UniqueHash));

            if (clientViewModel.ClientSocket != null && clientViewModel.ClientTable != null)
            {
                table.InTraffic = clientViewModel.ClientTable.InTraffic + clientViewModel.ClientSocket.InTraffic;
                table.OutTraffic = clientViewModel.ClientTable.OutTraffic + clientViewModel.ClientSocket.OutTraffic;
            }

            if (query.Count() > 0)
            {
                this.connection.Update(table);
            }
            else
            {
                this.connection.Insert(table);
            }
        }



        public List<ChatViewModel> SelectAllChats()
        {
            TableQuery<ChatTable> query = connection.Table<ChatTable>();

            List<ChatViewModel> viewModels = new List<ChatViewModel>();
            foreach (ChatTable table in query)
            {
                viewModels.Add(new ChatViewModel(table));
            }

            return viewModels;
        }

        public void InsertChat(ChatViewModel chatViewModel)
        {
            ChatTable table = ChatTable.Create(chatViewModel);
            this.connection.Insert(table);
        }

    }
}
