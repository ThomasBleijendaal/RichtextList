using System;
using RichText.Enums;

namespace RichText.Abstractions
{
    public interface IAppState
    {
        event EventHandler<ViewState> ViewStateChanged;

        ViewState State { get; }

        void ConfigureAuth(string url, string username, string password);

        void ConfigureBoard(string boardId);

        void ConfigureMeta(string projectId, string epicIssueType, string epicNamePropertyName, string userStoryIssueType, string subTaskIssueType);

        void ResetBoard();

        string Url { get; }

        string Username { get; }
        string Password { get; }

        string BoardId { get; }
        string ProjectId { get; }
        string EpicIssueType { get; }
        string EpicNamePropertyName { get; }
        string UserStoryIssueType { get; }
        string SubTaskIssueType { get; }
    }
}
