using System;

namespace CoreWebTinhTien.NHibernateSession
{
    public class SQLParam
    {
        private string _ParameName;

        private object _ParamValue;

        public string ParameName
        {
            get
            {
                return this._ParameName;
            }
            set
            {
                this._ParameName = value;
            }
        }

        public object ParamValue
        {
            get
            {
                return this._ParamValue;
            }
            set
            {
                this._ParamValue = value;
            }
        }

        public SQLParam(string mParamName, object mParamValue)
        {
            this._ParameName = mParamName;
            this._ParamValue = mParamValue;
        }
    }
}
