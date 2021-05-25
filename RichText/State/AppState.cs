using System;
using RichText.Abstractions;
using RichText.Enums;

namespace RichText.State
{
    public class AppState : IAppState
    {
        public ViewState State { get; private set;  }

        public string Url { get; private set; } = "";

        public string Username { get; private set; } = "";

        public string Password { get; private set; } = "";

        public string BoardId { get; private set; } = "";

        public string ProjectId { get; private set; } = "";

        public string EpicIssueType { get; private set; } = "";

        public string EpicNamePropertyName { get; private set; } = "";

        public string UserStoryIssueType { get; private set; } = "";

        public string SubTaskIssueType { get; private set; } = "";

        public event EventHandler<ViewState>? ViewStateChanged;

        public void ConfigureAuth(string url, string username, string password)
        {
            Url = url.Trim(new[] { ' ', '/' });
            Username = username;
            Password = password;

            BoardId = "";
            ProjectId = "";

            State = ViewState.NoBoardConfig;

            ViewStateChanged?.Invoke(this, State);
        }

        public void ConfigureBoard(string boardId)
        {
            BoardId = boardId;

            State = ViewState.BoardConfig;

            ViewStateChanged?.Invoke(this, State);
        }

        public void ConfigureMeta(string projectId, string epicIssueType, string epicNamePropertyName, string userStoryIssueType, string subTaskIssueType)
        {
            ProjectId = projectId;

            EpicIssueType = epicIssueType;
            EpicNamePropertyName = epicNamePropertyName;
            UserStoryIssueType = userStoryIssueType;
            SubTaskIssueType = subTaskIssueType;
        }

        public void ResetBoard()
        {
            State = ViewState.NoConfig;

            ViewStateChanged?.Invoke(this, State);
        }
    }
}
