using ConsensusModule.Sockets;
using ConsensusModule.States;

namespace ConsensusModule
{
    public class Raft
    {
        private ManualResetEvent _exitEvent = new ManualResetEvent(false);
      
        public readonly SocketClient SocketClient;
        public Raft(int nodeId, int port, string nodeUri, string[]? nodes = null)
        {
            NodeId = nodeId;
            NodeName = $"node_raft_{NodeId}";
            CurrentTerm = 0;
            NodeUri = nodeUri;
            Nodes = nodes;
            SocketClient = new SocketClient(nodeId, this);
            Port = port;
            
            State = new FollowerState(this);
            
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Iniciando raft node na porta {Port}...");
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Nodes informados {string.Join(", ", nodes.Select(node => node))}");
            State.StartElectionTimer();
        }
        public State State { get; private set; } 
        
        public int NodeId { get; private set; }
        private string NodeName { get; set; }
        public int CurrentTerm { get; private set; }
        public int LogIndex { get; private set; } = 0;
        private string NodeUri { get; set; }
        private string? LeaderUri { get; set; }
        private DateTime? LastLeaderPing { get; set; }
        
        private int Port { get; set; }
        public string[] Nodes { get; set; }

        #region Server
        public void StartServer()
        {
            SocketClient.Server("127.0.0.1", Port).GetAwaiter();
            _exitEvent.WaitOne();
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Raft node encerrado.");
        }
            
        public void Exit()
        {
            _exitEvent.Set(); 
        }
        #endregion

        #region  ChangeStates
        
        private void TransitionTo(State state)
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Trocando estado de {State.GetType().Name} para {state.GetType().Name}");
            this.State = state;
            this.State.SetContext(this);
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Estado trocado para {state.GetType().Name}");
        }
        
        public void Follower()
        {
            TransitionTo(new FollowerState(this));
        }
        
        public void Leader()
        {
            TransitionTo(new LeaderState(this));
        }

        public void Candidate()
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{State.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Temporizador de eleição expirou. Transitando para Candidato.");

            TransitionTo(new CandidateState(this));
        }

        #endregion


        public void AddTerm()
        {
            CurrentTerm += 1;
        }
    }
}
