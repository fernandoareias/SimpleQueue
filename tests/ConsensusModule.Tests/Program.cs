

using ConsensusModule;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"[*][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {args[0]}] - Pressione Enter para encerrar o programa.");
        Raft raft = new Raft(int.Parse(args[0]), int.Parse(args[1]), "http://localhost:5000", new string[1]{ args[2] });

        Thread raftThread = new Thread(() => raft.StartServer());
        raftThread.Start();

        
        Console.ReadLine();

        raft.Exit();
            
        raftThread.Join();

        Console.WriteLine($"[*][{DateTime.Now:yyyy-MM-dd HH:mm:ss}][PROCESSO {args[0]}] - Processo encerrado.");
    }
}


