using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

namespace Script.DB
{
    public class DBCommunicator : MonoBehaviour
    {
        #region UI_Reference
        [Header("UI")]
        [SerializeField] InputField Input_Query;
        [SerializeField] Text Text_DBResult;
        #endregion

        #region Field
        [Header("ConnectionInfo")] 
        [SerializeField]private string _ip = "127.0.0.1";
        [SerializeField] private string _dbName = "test";
        [SerializeField] private string _uid = "root";
        [SerializeField] private string _pwd = "1234";
        private static MySqlConnection _dbConnection;
        #endregion
    
        #region MonoBehavior Function
        private void Start()
        {
            OnStartConnectDB();
        }
        #endregion
    
        #region DB Function
        private void SendQuery(string queryStr, string tableName)
        {
            //Select
            if (queryStr.Contains("SELECT"))
            {
                DataSet dataSet = OnSelectRequest(queryStr, tableName);
                Text_DBResult.text = ReformatResult(dataSet);
            }
            //Create
            else if (queryStr.Contains("CREATE"))
            {
            
            }
            //Update 또는 Insert
            else
            {
                bool isSuccess = OnInsertOnUpdateRequest(queryStr, tableName);
                string resultStr = string.Empty;
                if (queryStr.Contains("INSERT"))
                {
                    resultStr = "INSERT ";
                }
                else if (queryStr.Contains("UPDATE"))
                {
                    resultStr = "UPDATE ";
                }
                resultStr += isSuccess ? "Success" : "Failure" ;
                Text_DBResult.text = resultStr;
            }
        }
        private string ReformatResult(DataSet dataSet)
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
                    resultStr += "\n";
                }
            }
            return resultStr;
        }
        public static DataSet OnSelectRequest(string query, string tableName)
        {
            try
            {
                MySqlCommand sqlCmd = new MySqlCommand();
                sqlCmd.Connection = _dbConnection;
                sqlCmd.CommandText = query;

                _dbConnection.Open();
            
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
        public static bool OnInsertOnUpdateRequest(string query, string tableName)
        {
            try
            {
                MySqlCommand sqlCmd = new MySqlCommand();
                sqlCmd.Connection = _dbConnection;
                sqlCmd.CommandText = query;

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
        private bool OnStartConnectDB()
        {
            string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectStr))
                {
                    _dbConnection = conn;
                    conn.Open();
                }
                Text_DBResult.text = "DB 연결을 성공했습니다!";
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"e: {e.ToString()}");
                Text_DBResult.text = "DB 연결 실패!!!!!!!!!!!!";
                return false;
            }
        }
        #endregion
    
        #region Button Event Function
        public void OnClick_SelectQuery()
        {
            Input_Query.text = @"SELECT U_Id, U_Name, U_Password FROM user_info;";
        }
        public void OnClick_InsertQuery()
        {
            Input_Query.text = @"INSERT INTO user_info (U_Id, U_Name, U_Password) VALUES ('', '', '');";
        }
        public void OnClick_UpdateQuery()
        {
            Input_Query.text = @"UPDATE user_info SET U_Password = '' WHERE U_Id = '';";
        }
        public void OnClick_CreateQuery()
        {
            Input_Query.text = "";
        }
        public void OnClick_SendQuery()
        {
            SendQuery(Input_Query.text, "user_info");
        }
        #endregion
        #region InputField Event Function
        public void OnSubmit_SendQuery()
        {
            SendQuery(Input_Query.text, "user_info");
        }
        #endregion
    }
}
