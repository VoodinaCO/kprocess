using KProcess.KL2.WebAdmin.Models.Survey;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public class QuestionnaireMapper
    {
        public static SurveyManageViewModel ToSurveyManageViewModel(
            IEnumerable<Survey> surveys
            )
        {
            var surveyManageViewModel = new SurveyManageViewModel();
            surveyManageViewModel.Surveys = surveys.Select(s => new SurveyViewModel
            {
                SurveyId = s.Id,
                Name = s.Name,
                SurveyItems = s.SurveyItems.Select(i => new SurveyItemViewModel
                {
                    SurveyId = i.SurveyId,
                    Number = i.Number,
                    Query = i.Query
                }).ToList()
            });
            return surveyManageViewModel;
        }

        public static SurveyViewModel ToSurveyViewModel(
            Survey survey)
        {
            var model = new SurveyViewModel
            {
                SurveyId = survey.Id,
                Name = survey.Name,
                SurveyItems = survey.SurveyItems.Select(i => new SurveyItemViewModel
                {
                    SurveyId = i.SurveyId,
                    Number = i.Number,
                    Query = i.Query
                }).ToList()
            };
            return model;
        }
    }
}