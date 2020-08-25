using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    public static class SqlExecutionResult
    {
        /// <summary>
        /// Crée une nouvelle instance de <see cref="SqlExecutionResult"/>
        /// </summary>
        /// <typeparam name="T">Le type de retour de la tache</typeparam>
        /// <param name="sqlConnection">La connection sql utilisée par la tache</param>
        /// <param name="task">La tache qui traite l'execution sql</param>
        /// <returns>La nouvelle instance de <see cref="SqlExecutionResult"/></returns>
        public static SqlExecutionResult<T> New<T>(SqlConnection sqlConnection, Task<T> task)
        {
            return new SqlExecutionResult<T>(sqlConnection, task);
        }
    }

    /// <summary>
    /// Définit le retour d'une commande Sql
    /// </summary>
    public class SqlExecutionResult<T>
    {
        private SqlConnection _sqlConnection = null;

        public SqlExecutionResult(SqlConnection sqlConnection, Task<T> task)
        {
            _sqlConnection = sqlConnection;
            this.Task = task;

            sqlConnection.Disposed += SqlConnectionDisposed;
            sqlConnection.InfoMessage += SqlConnectionInfoMessage;
        }

        private void SqlConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            if (SqlMessageSent != null)
            {
                this.SqlMessageSent(this, e);
            }
        }

        private void SqlConnectionDisposed(object sender, EventArgs e)
        {
            _sqlConnection.Disposed -= SqlConnectionDisposed;
            _sqlConnection.InfoMessage -= SqlConnectionInfoMessage;
            _sqlConnection = null;
        }

        /// <summary>
        /// Contient la tache qui se termine lorsque la commande a finit son execution
        /// </summary>
        public Task<T> Task { get; private set; }

        /// <summary>
        /// Se produit lorsque la connection Sql a intercepté un message du serveur
        /// </summary>
        public event EventHandler<SqlInfoMessageEventArgs> SqlMessageSent = null;

        public static implicit operator Task<T>(SqlExecutionResult<T> result)
        {
            return result.Task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        public Task<T> Start()
        {
            if(this.Task.Status == TaskStatus.Created)
            {
                this.Task.Start();
            }

            return this.Task;
        }

        /// <summary>
        /// Execute la tâche de façon synchrone et récupère le résultat
        /// </summary>
        /// <returns>Le resultat de l'execution de la tache</returns>
        public T GetResultSynchronously()
        {
            this.Task.Wait();
            return Task.Result;
        }
    }
}
