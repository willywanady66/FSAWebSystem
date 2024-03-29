﻿namespace FSAWebSystem.Models.ViewModels
{
    public class DataTableParamProposal
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        //public List<Column> columns { get; set; }
        public Search search { get; set; }  
        public List<Order> order { get; set; }
        public List<ProposalViewModel.ProposalInput> proposalInputs { get; set; }
        public string test { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
