﻿using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using static FSAWebSystem.Models.ViewModels.ProposalViewModel;

namespace FSAWebSystem.Services.Interface
{
    public interface IProposalService
    {

        public Task<ProposalData> GetProposalForView(int month, int year, int week, DataTableParamProposal param, Guid userId);
        //public Task<ProposalData> GetProposalForView(int month, int year, int week, int pageNo, Guid userId);
    }
}
