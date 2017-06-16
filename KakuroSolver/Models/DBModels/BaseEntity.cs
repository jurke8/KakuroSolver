using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KakuroSolver.Models.DBModels
{
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
    }
    public interface IBaseEntity
    {
        Guid Id { get; set; }
    }
}