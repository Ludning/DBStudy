using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

   [Header("ConnectionInfo")] 
   [SerializeField]private string _ip = "127.0.0.1";
   [SerializeField] private string _dbName = "test";
   [SerializeField] private string _uid = "root";
   [SerializeField] private string _pwd = "1234";
   private static MySqlConnection _dbConnection;
   
   
   //중요하지 않은
   private bool _isConnectTestComplete;

   private void Awake()
   {
       this.gameObject.SetActive(false);
   }


   private void SendQuary(string queryStr, string tableName)
   {
       //있으면 SELECT쿼리 호출
       if (queryStr.Contains("SELECT"))
       {
           DataSet dataSet = OnSelectRequest(queryStr, tableName);
           Text_DBResult.text = DeformatResult(dataSet);
       }
       //없으면 INSERT또는 UPDATE 쿼리 호출
       else
       {
           Text_DBResult.text = OnInsertOnUpdateRequest(queryStr, tableName) ? "성공" : "실패";
       }
   }

   public static bool OnInsertOnUpdateRequest(string query, string tableName)
   {
       try
       {
           MySqlCommand sqlCmd = new MySqlCommand();
           sqlCmd.Connection = _dbConnection;
           sqlCmd.CommandText = query;

           sqlCmd.Parameters.AddWithValue("@value1", "some_value");

           _dbConnection.Open();
           sqlCmd.ExecuteNonQuery();
           _dbConnection.Close();
           return true;
       }
       catch (Exception e)
       {
           return false;
       }
   }
   public static DataSet OnSelectRequest(string query, string tableName)
   {
       try
       {
           _dbConnection.Open();
           MySqlCommand sqlCmd = new MySqlCommand();
           sqlCmd.Connection = _dbConnection;
           sqlCmd.CommandText = query;

           MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
           DataSet dataSet = new DataSet();
           sd.Fill(dataSet, tableName);
           
           _dbConnection.Close();
           return dataSet;
       }
       catch (Exception e)
       {
           Debug.Log(e.ToString());
           return null;
       }
   }
   private bool ConnectTest()
   {
       string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";
       try
       {
           using (MySqlConnection conn = new MySqlConnection(connectStr))
           {
               _dbConnection = conn;
               conn.Open();
           }
           Text_Log.text = "DB 연결을 성공했습니다!";
           return true;
       }
       catch (Exception e)
       {
           Debug.LogWarning($"e: {e.ToString()}");
           Text_Log.text = "DB 연결 실패!!!!!!!!!!!!";
           return false;
       }
   }
   
   private string DeformatResult(DataSet dataSet)
   {
       string resultStr = string.Empty;

       foreach (DataTable table in dataSet.Tables)
       {
           foreach (DataRow row in table.Rows)
           {
               foreach (DataColumn column in table.Columns)
               {
                   resultStr += $"{column.ColumnName}: {row[column]}\n";
               }
           }
       }
       return resultStr;
   }


   #region UIEvent Function
    public void OnClick_TestDBConnect()
    {
        _isConnectTestComplete = ConnectTest();
    }
    public void OnSubmit_SendQuery()
    {
        if (_isConnectTestComplete == false)
        {
            Text_Log.text = "DB 연결을 먼저 시도해 주세요";
            return;
        }

        Text_Log.text = string.Empty;
        
        string query = string.IsNullOrWhiteSpace(Input_Query.text) ? "SELECT U_Name, U_Password FROM user_info" : Input_Query.text;
        SendQuary(query, "user_info");
    }
    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }
    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }
   #endregion
}
