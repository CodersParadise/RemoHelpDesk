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
            this.connection = new SQLiteConnection("RemoControl");
        }


        public void CreatTables()
        {
            this.connection.CreateTable<ClientTable>();
        }

        public ClientViewModel SelectClient(string identityName)
        {
            TableQuery<ClientTable> query = connection.Table<ClientTable>().Where(cli => cli.IdentityName.Equals(identityName));

            ClientTable table = query.FirstOrDefault();
            ClientViewModel viewModel = new ClientViewModel(table);

            return viewModel;
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

            TableQuery<ClientTable> query = connection.Table<ClientTable>().Where(cli => cli.IdentityName.Equals(table.IdentityName));

            if (query.Count() > 0)
            {
                this.connection.Update(table);
            }
            else
            {
                this.connection.Insert(table);
            }
        }


    }
}
