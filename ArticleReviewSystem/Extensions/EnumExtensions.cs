using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ArticleReviewSystem.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName(); 
            }
            catch (NullReferenceException)
            {
                if(enumValue == null)
                {
                    return string.Empty;
                }
                else
                {
                    return enumValue.ToString();
                }
                
            }

        }
    }
}