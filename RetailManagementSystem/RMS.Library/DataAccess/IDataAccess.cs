namespace RMS.Library.DataAccess
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        Task<int> SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void StartTransaction(string connectionStringName);
        Task<List<T>> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
        Task<int> SaveDataInTransaction<T>(string storedProcedure, T parameters);
        void CommitTransaction();
        void RollbackTransaction();
        void Dispose();
    }
}