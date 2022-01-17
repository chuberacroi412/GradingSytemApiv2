using GradingSytemApi.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Services.Interfaces
{
    public interface IScoreService
    {
        void UpdateScore(List<UpdateScoreModel> model, ref ErrorModel errors);
        byte[] ExportScoreComponent(Guid Id, ref ErrorModel errors);
        byte[] ExportScoreBoard(Guid Id, ref ErrorModel errors);
        void ImportScoreComponent(Guid Id, IFormFile file, ref ErrorModel errors);
        void ImportScoreBoard(Guid Id, IFormFile file, ref ErrorModel errors);
        void ChangeState(Guid Id, bool state, ref ErrorModel errors);
    }
}
