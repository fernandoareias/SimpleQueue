using System;
using System.Threading;
using System.Timers;
using ConsensusModule.Commands.Common;
using ConsensusModule.Interfaces;
using ConsensusModule.Logs;
using ConsensusModule.Sockets;
using ConsensusModule.States;

namespace ConsensusModule
{
    public class Raft
    {
        private ManualResetEvent _exitEvent = new ManualResetEvent(false);
        private System.Timers.Timer _electionTimer;
        private SocketClient _socketClient;
        public Raft(int nodeId, int port, string nodeUri, string[]? nodes = null)
        {
            NodeId = nodeId;
            NodeName = $"node_raft_{NodeId}";
            CurrentTerm = 0;
            NodeUri = nodeUri;
            Nodes = nodes;
            _socketClient = new SocketClient(nodeId);
            Port = port;
            
            CurrentState = FollowerState;
            
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Iniciando raft node na porta {Port}...");
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Nodes informados {string.Join(", ", nodes.Select(node => node))}");
            StartElectionTimer();
    
        }
        
        public IFollowerState FollowerState { get; private set; } = new FollowerState();
        public ICandidateState CandidateState { get; private set; } = new CandidateState();
        public ILeaderState LeaderState { get; private set; } = new LeaderState();
        public IRaftStates CurrentState { get; private set; } 
        
        private int NodeId { get; set; }
        private string NodeName { get; set; }
        private int CurrentTerm { get; set; }
        private string NodeUri { get; set; }
        private string? LeaderUri { get; set; }
        private DateTime? LastLeaderPing { get; set; }
        
        private int Port { get; set; }
        private string[] Nodes { get; set; }

        public void Exit()
        {
            _exitEvent.Set();
            _electionTimer?.Stop();
        }

        public void StartServer()
        {
            _socketClient.Server("127.0.0.1", Port).GetAwaiter();
            _exitEvent.WaitOne();
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Raft node encerrado.");
        }

        private void StartElectionTimer()
        {
            if (CurrentState == CandidateState)
                CurrentState = FollowerState;
            
            Random random = new Random();
            double timeout = random.Next(1500, 3001);

            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Iniciando a próxima eleição em {timeout}ms ...");

            _electionTimer = new System.Timers.Timer(timeout);
            _electionTimer.Elapsed += StartElection;
            _electionTimer.AutoReset = false; 
            _electionTimer.Start();
        }
        
        private void StartElection(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Iniciando a eleição.");
            if (Nodes.Length == 0)
            {
                Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Nenhum nó foi registrado, solicitando lideranca...");
                ChangeState(LeaderState);
                return;
            }

            foreach (var node in Nodes)
            {
                var n = node.Split(":");
                Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Enviando mensagem para o node {node} ");
                var response = _socketClient.Send($"{(int)ECommandType.REQUEST_VOTE}", n[0], int.Parse(n[1])).GetAwaiter();
                Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - {node} respondeu {response}");
            }
        }

        private void ChangeState(IRaftStates state)
        {
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Trocando estado de {CurrentState.GetType().Name} para {state.GetType().Name}");
            CurrentState = state;
            Console.WriteLine($"[+][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {NodeId}][{CurrentState.GetType().Name}][TERMO {CurrentTerm}][LOG INDEX - {0}] - Trocou o estado de {CurrentState.GetType().Name} para {state.GetType().Name}");
        }

    }
}
