using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Helpers
{
    public class Settings
    {
        public static IConfiguration Configuration { get; set; }

        public static string DEFAULT_ADMIN_ROLE_NAME
        {
            get
            {
                return GetAppConfig("DefaultAdmin:RoleName");
            }
        }

        public static string DEFAULT_ADMIN_EMAIL
        {
            get
            {
                return GetAppConfig("DefaultAdmin:Email");
            }
        }

        public static string DEFAULT_ADMIN_PASSWORD
        {
            get
            {
                return GetAppConfig("DefaultAdmin:Password");
            }
        }

        public static string DEFAULT_TEACHER_ROLE_NAME
        {
            get
            {
                return GetAppConfig("DefaultRole:TeacherRoleName");
            }
        }

        public static string DEFAULT_STUDENT_ROLE_NAME
        {
            get
            {
                return GetAppConfig("DefaultRole:StudentRoleName");
            }
        }

        public static string JWT_KEY
        {
            get
            {
                return GetAppConfig("JwtToken:JwtKey");
            }
        }

        public static double JWT_EXPIRE_DAY
        {
            get
            {
                try
                {
                    return Double.Parse(GetAppConfig("JwtToken:JwtExpireDays"));
                }
                catch(Exception e)
                {
                    return 0;
                }
                
            }
        }

        public static string JWT_ISSUER
        {
            get
            {
                return GetAppConfig("JwtToken:JwtIssuer");
            }
        }

        public static string SEND_EMAIL_ADDRESS
        {
            get
            {
                return GetAppConfig("SendEmailConfiguration:Email");
            }
        }

        public static string SEND_EMAIL_PASSWORD
        {
            get
            {
                return GetAppConfig("SendEmailConfiguration:Password");
            }
        }

        public static string SEND_EMAIL_NAME
        {
            get
            {
                return GetAppConfig("SendEmailConfiguration:Name");
            }
        }

        public static string SEND_EMAIL_SMTP_ADDRESS
        {
            get
            {
                return GetAppConfig("SendEmailConfiguration:SmtpAddress");
            }
        }
        private static string GetAppConfig(string val)
        {
            return Configuration.GetValue<string>(val) ?? string.Empty;
        }
    }
}
