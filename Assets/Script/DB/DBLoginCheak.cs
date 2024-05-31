using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.DB
{
    public class DBLoginCheak : MonoBehaviour
    {
        #region UI_Reference
        [Header("UI")]
        [SerializeField] InputField Input_Login_ID;
        [SerializeField] InputField Input_Login_Password;
        
        [SerializeField] InputField Input_SignUp_ID;
        [SerializeField] InputField Input_SignUp_Password;
        [SerializeField] InputField Input_SignUp_NickName;
        
        [SerializeField] InputField Input_ChangePassword_ID;
        [SerializeField] InputField Input_ChangePassword_Password;
        
        [SerializeField] Text Text_LoginResult;
        [SerializeField] Text Text_SignUpLog;
        [SerializeField] Text Text_ChangePasswordLog;
        
        [SerializeField] GameObject UI_SignUp;
        [SerializeField] GameObject UI_ChangePassword;
        #endregion

        #region Field
        [Header("ConnectionInfo")] 
        [SerializeField]private string _ip = "127.0.0.1";
        [SerializeField] private string _dbName = "test";
        [SerializeField] private string _uid = "root";
        [SerializeField] private string _pwd = "1234";
        [SerializeField] private string _tableName = "game_player";
        private static MySqlConnection _dbConnection;
        #endregion
    
        #region MonoBehavior Function
        private void Start()
        {
            OnStartConnectDB();
        }
        #endregion
    
        #region DB Function
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
                Text_LoginResult.text = "DB 연결을 성공했습니다!";
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"e: {e.ToString()}");
                Text_LoginResult.text = "DB 연결 실패!!!!!!!!!!!!";
                return false;
            }
        }
        
        private void SendLoginQuery(string queryStr, string tableName)
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);
            if (dataSet == null)
                Text_LoginResult.text = "로그인을 실패했습니다.";
            else
                Text_LoginResult.text = ReformatResult(dataSet);
        }
        private void SendSignUpQuery(string queryStr, string tableName)
        {
            Debug.LogWarning(queryStr);
            bool isSuccess = OnInsertOnUpdateRequest(queryStr, tableName);

            if (isSuccess == true)
            {
                UI_SignUp.SetActive(false);
            }
            else
            {
                Text_SignUpLog.text = "SignUp Failure";
            }
        }
        private void SendChangePasswordQuery(string queryStr, string tableName)
        {
            bool isSuccess = OnInsertOnUpdateRequest(queryStr, tableName);

            if (isSuccess == true)
            {
                UI_ChangePassword.SetActive(false);
            }
            else
            {
                Text_ChangePasswordLog.text = "ChangePassword Failure";
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
                        resultStr += $"{column.ColumnName}: {row[column]}\t";
                    }
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
        #endregion

        #region Button Activate UI Event Function
        public void OnClick_ActivateSignUpUI()
        {
            Text_SignUpLog.text = "";
            UI_SignUp.SetActive(true);
        }
        public void OnClick_ActivateChangePasswordUI()
        {
            Text_ChangePasswordLog.text = "";
            UI_ChangePassword.SetActive(true);
        }
        #endregion
        #region Button SendQuery Event Function
        public void OnClick_DBLogin()
        {
            string id = Input_Login_ID.text;
            string password = Input_Login_Password.text;
            
            string queryStr = $@"SELECT U_PlayerId, U_Password, U_NickName FROM {_tableName} WHERE U_PlayerId = '{id}' AND U_Password = '{password}';";
            
            SendLoginQuery(queryStr, _tableName);
        }
        public void OnClick_DBInsertSignUp()
        {
            string id = Input_SignUp_ID.text;
            string password = Input_SignUp_Password.text;
            string nickName = Input_SignUp_NickName.text;
            
            string queryStr = $@"INSERT INTO {_tableName} (U_PlayerId, U_Password, U_NickName) VALUES ('{id}', '{password}', '{nickName}');";
            
            SendSignUpQuery(queryStr, _tableName);
        }
        public void OnClick_DBUpdatePassword()
        {
            string id = Input_ChangePassword_ID.text;
            string password = Input_ChangePassword_Password.text;
            
            string queryStr = $@"UPDATE {_tableName} SET U_Password = '{password}' WHERE U_PlayerId = '{id}';";
            
            SendChangePasswordQuery(queryStr, _tableName);
        }
        #endregion
    }
}
