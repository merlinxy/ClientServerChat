using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClientServerChat.ChatServiceReference;
using Prism.Commands;

namespace ClientServerChat
{
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        UseSynchronizationContext =false)]
    class MainWindowViewModel : ChatServiceReference.IChatServiceInboundCallback, INotifyPropertyChanged
    {
        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private string _name;

        public string ChatContent
        {
            get { return _chatContent; }
            set { _chatContent = value; OnPropertyChanged(); }
        }
        private string _chatContent;

        public string SendContent
        {
            get { return _sendContent; }
            set { _sendContent = value; OnPropertyChanged(); }
        }
        private string _sendContent;

        public Boolean JoinButtonEnabled
        {
            get { return _joinButtonEnabled; }
            set { _joinButtonEnabled = value; OnPropertyChanged(); }
        }
        private Boolean _joinButtonEnabled;
        public Boolean SendButtonEnabled
        {
            get { return _sendButtonEnabled; }
            set { _sendButtonEnabled = value; OnPropertyChanged(); }
        }
        private Boolean _sendButtonEnabled;
        public DelegateCommand JoinCommand { get; set; }
        public DelegateCommand SendCommand { get; set; }
        public DelegateCommand LeaveCommand { get; set; }
        #endregion

        ChatServiceInboundClient _client = null;

        public MainWindowViewModel()
        {
            //initialise buttons
            JoinButtonEnabled = true;
            SendButtonEnabled = false;

            JoinCommand = new DelegateCommand(join);
            SendCommand = new DelegateCommand(send);
            LeaveCommand = new DelegateCommand(leave);
            _client = new ChatServiceInboundClient(new System.ServiceModel.InstanceContext(this));
            _client.Open();
        }

        private void WriteMessage(string message)
        {
            string format = (ChatContent == null || ChatContent.Length > 0) ? "{0}\r\n{1} {2}" : "{0}{1} {2}";
            this.ChatContent = String.Format(format, ChatContent, DateTime.Now.ToShortTimeString(), message);
        }

        private void join()
        {
            //contact the service.
            _client.JoinTheConversation(Name);

            // change the button states
            JoinButtonEnabled = false;
            SendButtonEnabled = true;
        }
        private void send()
        {          
            // Forward the message to the service
            _client.ReceiveMessage(Name, null, SendContent);
            SendContent = "";          
        }
        private void leave()
        {
            _client.LeaveTheConversation(Name);
            JoinButtonEnabled = true;
            SendButtonEnabled = false;
        }
        public void NotifyUserJoinedTheConversation(string userName)
        {
            this.WriteMessage(String.Format("[{0}] has joined the conversation.", userName));
        }

        public void NotifyUserLeftTheConversation(string userName)
        {
            this.WriteMessage(String.Format("[{0}] has left the conversation.", userName));
        }

        public void NotifyUserOfMessage(string userName, string userMessage)
        {
            this.WriteMessage(String.Format("[{0}]: {1}", userName.ToUpper(), userMessage));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String caller = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
