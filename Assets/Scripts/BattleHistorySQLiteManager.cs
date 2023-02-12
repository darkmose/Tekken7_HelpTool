using Mono.Data.SqliteClient;
using SQLiter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;


namespace Database
{
    public class BattleHistorySQLiteManager : MonoBehaviour
    {
		private static string _sqlDBLocation = "";
		private const string SQL_DB_NAME = "BattleHistory";
		private const string SQL_TOURNAMENT_TABLE_NAME = "TournamentData";
		private const string SQL_PLAYER_TABLE_NAME = "PlayerData";
		private const string SQL_CHARACTER_TABLE_NAME = "CharacterData";

		private const string COL_DATE = "Date";
		private const string COL_TOURNAMENT_OVER = "TournamentOver";
		private const string COL_WINNER_NAME = "WinnerName";
		private const string COL_TOURNAMENT_ID = "TournamentID";
		private const string COL_PLAYER_ID = "PlayerID";
		private const string COL_PLAYER_WIN_COUNT = "PlayerWinCount";
		private const string COL_PLAYER_LOSE_COUNT = "PlayerLoseCount";
		private const string COL_PLAYER_WINRATE = "PlayerWinRate";
        private const string COL_PLAYER_NAME = "PlayerName";
		private const string COL_CHARACTER_ID = "CharacterID";
		private const string COL_CHARACTER_DEAD = "CharacterDead";
		private const string COL_CHARACTER_WIN_COUNT = "CharacterWinCount";
		private const string COL_CHARACTER_PERFECT_COUNT = "CharacterPerfectCount";
        private const string COL_CHARACTER_NAME = "CharacterName";
        private IDbConnection _connection = null;
		private IDbCommand _command = null;
		private IDataReader _reader = null;
		private string _sqlString;

		public bool _createNewTable = false;

		void Awake()
		{
			_sqlDBLocation = "URI=file:" + SQL_DB_NAME + ".db";
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

			_command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + SQL_TOURNAMENT_TABLE_NAME + "'";
			_reader = _command.ExecuteReader();
			if (!_reader.Read())
			{
				Debug.Log("SQLiter - Could not find SQLite table " + SQL_TOURNAMENT_TABLE_NAME);
				_createNewTable = true;
			}
			_reader.Close();

			if (_createNewTable)
			{
				Debug.Log("SQLiter - Dropping old SQLite table if Exists: " + SQL_TOURNAMENT_TABLE_NAME);

				_command.CommandText = "DROP TABLE IF EXISTS " + SQL_TOURNAMENT_TABLE_NAME;
				_command.ExecuteNonQuery();

				Debug.Log("SQLiter - Creating new SQLite table: " + SQL_TOURNAMENT_TABLE_NAME);

				_sqlString = "CREATE TABLE IF NOT EXISTS " + SQL_TOURNAMENT_TABLE_NAME + " (" +
					COL_TOURNAMENT_ID + " UNSIGNED TINYINT(255) UNIQUE, " +
					COL_DATE + " TINYTEXT, " +
					COL_TOURNAMENT_OVER + " BOOL, " +
					COL_WINNER_NAME + " TINYTEXT)"; 
				_command.CommandText = _sqlString;
				_command.ExecuteNonQuery();

                _command.CommandText = "DROP TABLE IF EXISTS " + SQL_PLAYER_TABLE_NAME;
                _command.ExecuteNonQuery();

                _sqlString = "CREATE TABLE IF NOT EXISTS " + SQL_PLAYER_TABLE_NAME + " (" +
                    COL_PLAYER_ID + " UNSIGNED TINYINT(255) UNIQUE, " +
                    COL_TOURNAMENT_ID + " UNSIGNED TINYINT(255), " +
					COL_PLAYER_NAME + " TINYTEXT, " +
                    COL_PLAYER_WIN_COUNT + " UNSIGNED TINYINT(255), " +
                    COL_PLAYER_LOSE_COUNT + " UNSIGNED TINYINT(255), " +
                    COL_PLAYER_WINRATE + " FLOAT(3, 1))";
                _command.CommandText = _sqlString;
                _command.ExecuteNonQuery();

                _command.CommandText = "DROP TABLE IF EXISTS " + SQL_CHARACTER_TABLE_NAME;
                _command.ExecuteNonQuery();

                _sqlString = "CREATE TABLE IF NOT EXISTS " + SQL_CHARACTER_TABLE_NAME + " (" +
                    COL_CHARACTER_ID + " UNSIGNED TINYINT(255) UNIQUE, " +
                    COL_PLAYER_ID + " UNSIGNED TINYINT(255), " +
                    COL_TOURNAMENT_ID + " UNSIGNED TINYINT(255), " +
                    COL_CHARACTER_DEAD + " BOOL, " +
                    COL_CHARACTER_WIN_COUNT + " UNSIGNED TINYINT(255), " +
                    COL_CHARACTER_PERFECT_COUNT + " UNSIGNED TINYINT(255), " +
                    COL_CHARACTER_NAME + " TINYTEXT)";
                _command.CommandText = _sqlString;
                _command.ExecuteNonQuery();
            }

			_connection.Close();
		}

		public void InsertTournament(string tournamentID, string dateTime, string tournamentOver, string winnerName)
        {
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TOURNAMENT_TABLE_NAME
				+ " ("
				+ COL_TOURNAMENT_ID+ ","
				+ COL_DATE + ","
				+ COL_TOURNAMENT_OVER + ","
				+ COL_WINNER_NAME + ") VALUES ("
				+ "'" + tournamentID + "',"
				+ "'" + dateTime + "',"
				+ "'" + tournamentOver + "',"
				+ "'" + winnerName + "');";

			ExecuteNonQuery(_sqlString);
		}

		public void InsertPlayer(string tournamentID, string playerID, string winCount, string loseCount, string winRate, string name)
        {
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TOURNAMENT_TABLE_NAME
				+ " ("
				+ COL_PLAYER_ID + ","
				+ COL_TOURNAMENT_ID + ","
				+ COL_PLAYER_NAME + ","
				+ COL_PLAYER_WIN_COUNT + ","
				+ COL_PLAYER_LOSE_COUNT+ ","
				+ COL_PLAYER_WINRATE
				+ ") VALUES ("
				+ "'" + playerID + "',"
				+ "'" + tournamentID + "',"
				+ "'" + name + "',"
				+ "'" + winCount + "',"
				+ "'" + loseCount + "',"
				+ "'" + winRate + "');";

			ExecuteNonQuery(_sqlString);
		}

		public void InsertCharacter(string tournamentID, string playerID, string characterID, string isDead, string winCount, string perfectCount, string characterName)
        {
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TOURNAMENT_TABLE_NAME
			+ " ("
			+ COL_CHARACTER_ID + ","
			+ COL_TOURNAMENT_ID + ","
			+ COL_PLAYER_ID + ","
			+ COL_CHARACTER_DEAD + ","
			+ COL_CHARACTER_WIN_COUNT + ","
			+ COL_CHARACTER_PERFECT_COUNT + ","
			+ COL_CHARACTER_NAME
			+ ") VALUES ("
			+ "'" + characterID + "',"
			+ "'" + tournamentID + "',"
			+ "'" + playerID + "',"
			+ "'" + isDead + "',"
			+ "'" + winCount + "',"
			+ "'" + perfectCount + "',"
			+ "'" + characterName + "');";

			ExecuteNonQuery(_sqlString);
		}		

		public int GetLastTournamentIndex()
        {
			int lastIndex = 0;
			_connection.Open();
			_command.CommandText = $"SELECT COUNT(*) FROM {SQL_TOURNAMENT_TABLE_NAME}";
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				lastIndex = _reader.GetInt16(0) - 1;
			}
			_reader.Close();
			_connection.Close();
			return lastIndex;
		}

		public int GetLastPlayerIndex(int tournamentID)
        {
			int lastIndex = 0;
			_connection.Open();
			_command.CommandText = $"SELECT COUNT(*) FROM {SQL_PLAYER_TABLE_NAME} WHERE {COL_TOURNAMENT_ID} = {tournamentID}";
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				lastIndex = _reader.GetInt16(0) - 1;
			}
			_reader.Close();
			_connection.Close();
			return lastIndex;
		}

		public List<TournamentDataDescriptor> LoadTournamentsRange(int startID, int lastID)
        {
			var tournamentList = new List<TournamentDataDescriptor>();

			_connection.Open();
			_command.CommandText = $"SELECT * FROM {SQL_TOURNAMENT_TABLE_NAME} WHERE {COL_TOURNAMENT_ID} BETWEEN {startID} AND {lastID}";
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				var tournamentData = new TournamentDataDescriptor();
				tournamentData.TournamentID = _reader.GetInt16(0);
				tournamentData.Date = _reader.GetString(1);
				tournamentData.IsTournamentOver = _reader.GetBoolean(2);
				tournamentData.Winner = _reader.GetString(3);
				tournamentList.Add(tournamentData);
			}
			_reader.Close();
			_connection.Close();
			return tournamentList;
        }

		public List<PlayerDataDescriptor> LoadPlayersFromTournament(int tournamentID)
        {
			var playerDatas = new List<PlayerDataDescriptor>();

			_connection.Open();
			_command.CommandText = $"SELECT * FROM {SQL_PLAYER_TABLE_NAME} WHERE {COL_TOURNAMENT_ID} = {tournamentID}";
			_reader = _command.ExecuteReader();

			while (_reader.Read())
			{
				var playerData = new PlayerDataDescriptor();
				playerData.PlayerID = _reader.GetInt16(0);
				playerData.PlayerName = _reader.GetString(2);
				playerData.WinCount = _reader.GetInt16(3);
				playerData.LoseCount = _reader.GetInt16(4);
				playerData.WinRate = _reader.GetFloat(5);
				playerDatas.Add(playerData);
			}
			_reader.Close();
			_connection.Close();
			return playerDatas;
		}

		public List<CharacterDataDescriptor> LoadCharactersOfPlayerFromTournament(int tournamentID, int playerID)
        {
			var characterDatas = new List<CharacterDataDescriptor>();
			_connection.Open();
			_command.CommandText = $"SELECT * FROM {SQL_CHARACTER_TABLE_NAME} WHERE {COL_TOURNAMENT_ID} = {tournamentID} AND {COL_PLAYER_ID} = {playerID}";
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				var characterData = new CharacterDataDescriptor();
				characterData.CharacterID = _reader.GetInt16(0);
				characterData.isDead = _reader.GetBoolean(3);
				characterData.WinCount = _reader.GetInt16(4);
				characterData.PerfectCount = _reader.GetInt16(5);
				characterData.CharacterName = _reader.GetString(6);
				characterDatas.Add(characterData);
			}
			_reader.Close();
			_connection.Close();
			return characterDatas;
		}

		private void ExecuteNonQuery(string commandText)
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

	public class TournamentDataDescriptor
    {
		public int TournamentID;
		public string Date;
		public bool IsTournamentOver;
		public string Winner;
		public List<PlayerDataDescriptor> PlayerDatas;
	}

	public class PlayerDataDescriptor
    {
		public int PlayerID;
		public int WinCount;
		public int LoseCount;
		public float WinRate;
		public string PlayerName;
		public List<CharacterDataDescriptor> CharactersDatas;
    }

	public class CharacterDataDescriptor
    {
		public int CharacterID;
		public bool isDead;
		public int WinCount;
		public int PerfectCount;
		public string CharacterName;
    }
}