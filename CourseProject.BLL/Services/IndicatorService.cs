﻿using CourseProject.BLL.Interfaces;
using CourseProject.BLL.Models;
using CourseProject.Core.Entities;
using CourseProject.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public class IndicatorService : IIndicatorService
    {
        private readonly CourseProjectDbContext context;

        public IndicatorService(CourseProjectDbContext context)
        {
            this.context = context;
        }

        public async Task<Guid> CreateIndicator(IndicatorModel model)
        {
            var indicator = new Indicator()
            {
                Description = model.Description,
                X = model.X,
                Y = model.Y,
                Value = model.Value,
                Unit = model.Unit,
                Name = model.Name
            };

            context.Add(indicator);
            await context.SaveChangesAsync();

            return indicator.Id;
        }

        public async Task DeleteIndicator(Guid id)
        {
            var indicator = await context.Indicators.FirstOrDefaultAsync(x => x.Id == id);

            if (indicator is null)
            {
                return;
            }

            context.Remove(indicator);

            await context.SaveChangesAsync();
        }

        public async Task<List<IndicatorModel>> GetIndicators()
        {
            var indicators = await context.Indicators
                                    .Include(i => i.IndicatorValues)
                                    .ToListAsync();

            var indicatorModels = indicators.Select(model => new IndicatorModel()
            {
                Id = model.Id,
                Description = model.Description,
                Value = model.Value,
                X = model.X,
                Y = model.Y,
                Unit = model.Unit,
                Name = model.Name,
                IndicatorValues = model.IndicatorValues.Select(v => v.Value).ToList(),
            }).ToList();

            return indicatorModels;
        }

        public async Task UpdateIndicatorValue(UpdateIndicatorValueModel model)
        {
            var indicator = await context.Indicators
                .Include(i => i.IndicatorValues)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (indicator is null)
            {
                throw new Exception("Indicator null exception");
            }

            var indicatorValue = new IndicatorValue()
            {
                Value = model.Value,
                DateTime = DateTime.Now
            };

            indicator.IndicatorValues.Add(indicatorValue);

            await context.SaveChangesAsync();
        }
    }
}