using GradingSytemApi.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Common.Helpers
{
    public static class UtilService
    {
		public static void CheckImportExcelFilleType(IFormFile file, ref ErrorModel error)
		{
			if (file == null || file.Length <= 0)
			{
				error.Add("File is empty");
			}
			else if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
			{
				error.Errors.Add("File is not excel file");
			}
		}
	}
}
