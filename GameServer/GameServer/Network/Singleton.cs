using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUsingProtobuf
{
    public class Singleton<T>
    {
        private static T mInstance;

        public static T instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = Activator.CreateInstance<T>();
                }

                return mInstance;
            }
        }
    }
}
