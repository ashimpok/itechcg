using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ITechcg.Dal
{
    /// <summary>
    /// Class wraps SqlCommand so that SqlCommand will not be passed explicitly without being created by the executor.
    /// Class also provides easy way to create parameters with different overloaded methods
    /// </summary>
    /// <remarks>Class is also created by TaskParm derieved classes to return a sql command for the task.</remarks>
    public class TaskSqlCommand
    {
        private SqlCommand _command;
        
        internal TaskSqlCommand(SqlCommand command)
        {
            _command = command;            
        }

        internal TaskSqlCommand(SqlCommand command, SqlTransaction transaction)
        {
            _command = command;
            _command.Transaction = transaction;
        }

        public SqlCommand Command
        {
            get { return _command; }
        }

        public void AttachParameter(SqlParameter[] sqlParamaters)
        {
            SqlParameterCollection parms = _command.Parameters;
            parms.AddRange(sqlParamaters);
        }

        public void CreateParameter(string paramName, bool? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;
            parm.SqlDbType = SqlDbType.Bit;
            if (value.HasValue)
                parm.Value = value.Value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateParameter(string paramName, int? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;            
            parm.SqlDbType = SqlDbType.Int;
            if (value.HasValue && value.Value != int.MinValue)
            {                
                parm.Value = value.Value;
            }
            else
                parm.Value = System.DBNull.Value;

            parms.Add(parm);
        }


        public void CreateParameter(string paramName, StringBuilder value)
        {
            CreateParameter(paramName, value, false);
        }

        /// <summary>
        /// This adds a string parameter value with an option to preserve the case
        /// of the string.
        /// </summary>
        /// <param name="preserveCase">If <c>true</c> preserves the case otherwise all will be converted to upper case.</param>
        public void CreateParameter(string paramName, StringBuilder value, bool preserveCase)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;
            parm.SqlDbType = SqlDbType.VarChar;

            if (value != null || value.Length > 0)
            {
                if (preserveCase)
                    parm.Value = value.ToString();
                else
                    parm.Value = value.ToString().ToUpper();
            }
            else
            {
                parm.Value = System.DBNull.Value;
            }

            parms.Add(parm);
        }

        /// <summary>
        /// This adds a string parameter value in all upper case.
        /// Use the override and pass true to preserveCase parameter,
        /// if you would like to preserve the case of the string.
        /// </summary>
        public void CreateParameter(string paramName, string value)
        {
            CreateParameter(paramName, value, false);
        }

        /// <summary>
        /// This adds a string parameter value with an option to preserve the case
        /// of the string.
        /// </summary>
        /// <param name="preserveCase">If <c>true</c> preserves the case otherwise all will be converted to upper case.</param>
        public void CreateParameter(string paramName, string value, bool preserveCase)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;
            parm.SqlDbType = SqlDbType.VarChar;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (preserveCase)
                    parm.Value = value.Trim();
                else
                    parm.Value = value.ToUpper().Trim();
            }
            else
            {
                parm.Value = System.DBNull.Value;
            }

            parms.Add(parm);
        }

        public void CreateParameter(string paramName, Enum anEnum)
        {
            string eName = Enum.GetName(anEnum.GetType(), anEnum);
            if (eName == "UNSET")
                eName = null;                
            
            CreateParameter(paramName, eName);
        }

        public void CreateParameter(string paramName, DateTime? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;
        
            parm.SqlDbType = SqlDbType.DateTime;
            //Min value is passed when DateTime instance is passed to DateTime?.
            //DateTime being a value type always passes 1/1/0001
            if (value.HasValue && value.Value.CompareTo(DateTime.MinValue) != 0)
            {
                parm.Value = value.Value;
            }
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateParameter(string paramName, Double? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.Decimal;
            if (value.HasValue)
                parm.Value = value.Value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateParameter(string paramName,  Decimal? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.Decimal;
            if (value.HasValue)
                parm.Value = value.Value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateParameter(string paramName, byte[] value, int length)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.Image;
            parm.Size = length;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;

            parms.Add(parm);
        }
        
        public void CreateTextParameter(string paramName, string value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.Text;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateVarBinaryParameter(string paramName, byte[] value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.VarBinary;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateNTextParameter(string paramName, string value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.NVarChar;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateBigIntParameter(string paramName, int? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.BigInt;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateSmallMoneyParameter(string paramName, Decimal? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.SmallMoney;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }

        public void CreateMoneyParameter(string paramName, Decimal? value)
        {
            SqlParameterCollection parms = _command.Parameters;
            SqlParameter parm = _command.CreateParameter();
            parm.ParameterName = paramName;

            parm.SqlDbType = SqlDbType.Money;
            if (value != null)
                parm.Value = value;
            else
                parm.Value = System.DBNull.Value;
            parms.Add(parm);
        }
    }
}
