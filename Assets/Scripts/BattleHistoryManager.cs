using Mono.Data.SqliteClient;
using SQLiter;
using System.Collections;
using System.Data;
using System.Text;
using UnityEngine;


namespace Database
{
    public class BattleHistoryManager : MonoBehaviour
    {
		private BattleHistoryManager _innerInstance;
		private static string _sqlDBLocation = "";
		private const string SQL_DB_NAME = "BattleHistory";
		private const string SQL_TABLE_NAME = "Battle";
		private const string COL_WORD = "Word";
		private const string COL_DEFINITION = "Definition";
		private const string COL_DATE = "Date";

		private IDbConnection _connection = null;
		private IDbCommand _command = null;
		private IDataReader _reader = null;
		private string _sqlString;

		public bool _createNewTable = false;

		void Awake()
		{
			_sqlDBLocation = "URI=file:" + SQL_DB_NAME + ".db";

			_innerInstance = this;
			SQLiteInit();
		}

		void OnDestroy()
		{
			SQLiteClose();
		}

		public void RunAsyncInit()
		{
			LoomManager.Loom.QueueOnMainThread(() =>
			{
				SQLiteInit();
			});
		}

		private void SQLiteInit()
		{
			Debug.Log("SQLiter - Opening SQLite Connection at " + _sqlDBLocation);
			_connection = new SqliteConnection(_sqlDBLocation);
			_command = _connection.CreateCommand();
			_connection.Open();

			_command.CommandText = "PRAGMA journal_mode = WAL;";
			_command.ExecuteNonQuery();
			_command.CommandText = "PRAGMA journal_mode";
			_reader = _command.ExecuteReader();
			_reader.Close();

			_command.CommandText = "PRAGMA synchronous = OFF";
			_command.ExecuteNonQuery();
			_command.CommandText = "PRAGMA synchronous";
			_reader = _command.ExecuteReader();		
			_reader.Close();

			_command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + SQL_TABLE_NAME + "'";
			_reader = _command.ExecuteReader();
			if (!_reader.Read())
			{
				Debug.Log("SQLiter - Could not find SQLite table " + SQL_TABLE_NAME);
				_createNewTable = true;
			}
			_reader.Close();

			if (_createNewTable)
			{
				Debug.Log("SQLiter - Dropping old SQLite table if Exists: " + SQL_TABLE_NAME);

				_command.CommandText = "DROP TABLE IF EXISTS " + SQL_TABLE_NAME;
				_command.ExecuteNonQuery();

				Debug.Log("SQLiter - Creating new SQLite table: " + SQL_TABLE_NAME);

				_sqlString = "CREATE TABLE IF NOT EXISTS " + SQL_TABLE_NAME + " (" +
					COL_WORD + " TEXT UNIQUE, " +
					COL_DEFINITION + " TEXT)";
				_command.CommandText = _sqlString;
				_command.ExecuteNonQuery();
			}

			_connection.Close();
		}

		public void InsertWord(string name, string definition)
		{
			name = name.ToLower();
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TABLE_NAME
				+ " ("
				+ COL_WORD + ","
				+ COL_DEFINITION
				+ ") VALUES ("
				+ "'" + name + "'," 
				+ "'" + definition + "');";

			ExecuteNonQuery(_sqlString);
		}

		public void GetAllWords()
		{
			StringBuilder sb = new StringBuilder();

			_connection.Open();

			_command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME;
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				sb.Length = 0;
				sb.Append(_reader.GetString(0)).Append(" ");
				sb.Append(_reader.GetString(1)).Append(" ");
				sb.AppendLine();
			}
			_reader.Close();
			_connection.Close();
		}

		public string GetDefinitation(string value)
		{
			return QueryString(COL_DEFINITION, value);
		}

		public string QueryString(string column, string value)
		{
			string text = "Not Found";
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_WORD + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				text = _reader.GetString(0);
			else
				Debug.Log("QueryString - nothing to read...");
			_reader.Close();
			_connection.Close();
			return text;
		}

		public void SetValue(string column, int value, string wordKey)
		{
			ExecuteNonQuery("UPDATE OR REPLACE " + SQL_TABLE_NAME + " SET " + column + "='" + value + "' WHERE " + COL_WORD + "='" + wordKey + "'");
		}

		public void DeleteWord(string wordKey)
		{
			ExecuteNonQuery("DELETE FROM " + SQL_TABLE_NAME + " WHERE " + COL_WORD + "='" + wordKey + "'");
		}

		public void ExecuteNonQuery(string commandText)
		{
			_connection.Open();
			_command.CommandText = commandText;
			_command.ExecuteNonQuery();
			_connection.Close();
		}

		private void SQLiteClose()
		{
			if (_reader != null && !_reader.IsClosed)
				_reader.Close();
			_reader = null;

			if (_command != null)
				_command.Dispose();
			_command = null;

			if (_connection != null && _connection.State != ConnectionState.Closed)
				_connection.Close();
			_connection = null;
		}		
	}
}