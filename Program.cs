
namespace Vadu_Testesana_AL
{
    internal class Program
    {

        //======================================================
        //Programma ir izstrādāta vadu testēšanai.
        //Tā ir saīsināta versija, kurā jau iepriekš ir izveidoti vārti ar noteiktām vērtībām
        //un lietotājam atliek vienīgi izveidot savienojumus un pārbaudīt to darbību. 
        //Programmas mērķis ir vienkārši izprast, kā darbojas datu pārraide.
        //Tās izstrādes laikā galvenā koncepcija kļuva skaidra, un tā tiks pārnesta uz WPF programmu. 
        //Vienīgā kļūda, kuru es neesmu novērsis
        //(un neplānoju to darīt, jo tā paliks konsolē, un jau pašā WPF programmā skaitīšanas sistēma tiks pārstrādāta),
        //ir tas, ka vārtiem un vadam ir kopīgs skaitītāja ID. 
        static void Main(string[] args)
        {
            // ─── IEPRIEKŠ IZVEIDOTIE VĀRTI ───────────────────────────
            // Gate A — AND ar 2 ieejām (true, false)
            var gateA = new AndGate();
            gateA.Inputs.Add(new InputPort { Data = true });
            gateA.Inputs.Add(new InputPort { Data = false });
            gateA.Process();

            // Gate B — OR ar 3 ieejām (false, false, true)
            var gateB = new OrGate();
            gateB.Inputs.Add(new InputPort { Data = false });
            gateB.Inputs.Add(new InputPort { Data = false });
            gateB.Inputs.Add(new InputPort { Data = true });
            gateB.Process();

            // Gate C — NOT ar 1 ieeju (true)
            var gateC = new NotGate();
            gateC.Inputs.Add(new InputPort { Data = true });
            gateC.Process();

            // Gate D — XOR ar 4 ieejām (true, true, true, false)
            var gateD = new XorGate();
            gateD.Inputs.Add(new InputPort { Data = true });
            gateD.Inputs.Add(new InputPort { Data = true });
            gateD.Inputs.Add(new InputPort { Data = true });
            gateD.Inputs.Add(new InputPort { Data = false });
            gateD.Process();

            var gates = new List<Gate> { gateA, gateB, gateC, gateD };

            // ─── 1. IZVADE — tikai vārti ─────────────────────────────
            Console.WriteLine("══════════════════════════════════");
            Console.WriteLine("  VĀRTI");
            Console.WriteLine("══════════════════════════════════");
            Console.WriteLine($"{"ID",-5} {"Tips",-8} {"Ieejas",-30} {"Izeja"}");
            Console.WriteLine(new string('─', 54));

            foreach (var gate in gates)
            {
                string inputs = string.Join(", ", gate.Inputs.Select((p, i) => $"P{i + 1}={(p.Data ? 1 : 0)}"));
                Console.WriteLine($"{gate.Id,-5} {gate.Name,-8} {inputs,-30} {(gate.Output.Data ? 1 : 0)}");
            }

            // ─── 2. IZVADE — vārti + vadi ────────────────────────────
            Console.WriteLine();
            Console.WriteLine("══════════════════════════════════");
            Console.WriteLine("  VĀRTI + VADI");
            Console.WriteLine("══════════════════════════════════");

            

            Console.WriteLine("(vadu kods vel nav pievienots)");
            Console.WriteLine();



            bool running = true;
            WireService service = new WireService();
            while (running)
            {
                
                Console.WriteLine("══════════════════════════════════");
                Console.WriteLine("  VADU IZVĒLNE");
                Console.WriteLine("══════════════════════════════════");
                Console.WriteLine("1. Savienot vadu");
                Console.WriteLine("2. Atvienot vadu");
                Console.WriteLine("3. Propagēt visus vadus");
                Console.WriteLine("4. Parādīt visus vadus");
                Console.WriteLine("5. Parādīt vārtus");
                Console.WriteLine("0. Iziet");
                Console.WriteLine("══════════════════════════════════");
                Console.Write("Izvēle: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Wire newwire = new Wire();
                        
                        Console.Write("Kuram Gate output portam izveidot sakuma savienojumu. "); 
                        int gateidO = int.Parse(Console.ReadLine());

                        Gate foundgateO = gates.FirstOrDefault(g => g.Id == gateidO);

                        if (foundgateO == null)
                        {
                            Console.WriteLine("Vārti nav atrasti!");
                            break;
                        }


                        Console.Write("Kuram Gate input portam izveidot sakuma savienojumu. ");
                        int gateidI = int.Parse(Console.ReadLine());
                        Gate foundgateI = gates.FirstOrDefault(g => g.Id == gateidI);

                        if (foundgateI == null || foundgateO == foundgateI)
                        {
                            Console.WriteLine("Vārti nav atrasti vai ir vienādi!");
                            break;
                        }
                        


                        Console.WriteLine("Pieejamie ieejas porti:");
                        for (int i = 0; i < foundgateI.Inputs.Count; i++)
                        {
                            Console.WriteLine($"  [{i}] P{i + 1} = {(foundgateI.Inputs[i].Data ? 1 : 0)}");
                        }
                        Console.Write("Porta izvele: ");
                        int portidI = int.Parse(Console.ReadLine());

                        if ( portidI < 0 || portidI >= foundgateI.Inputs.Count)
                        {
                            Console.WriteLine("Nepareiza porta izvēle!");
                            break;
                        }
                        InputPort portI = foundgateI.Inputs[portidI];
                        


                        service.Connect(newwire, foundgateO, portI, foundgateI );
                        break;
                    case "2": // Disconnect

                        var wires = service.GetAll();
                        Console.WriteLine("wire ID        StartPort      EndPort");
                        for (int i = 0; i < wires.Count; i++)
                        {
                            Wire wire = wires[i];
                            Console.Write("     " + wire.Id);
                            Console.Write("     " + (wire.StartPort.Data ? 1 : 0));
                            Console.Write("     " + (wire.EndPort.Data ? 1 : 0));
                            Console.WriteLine();
                        }
                        Console.Write("Kuru vadu dzest:  ");
                        service.Disconnect();

                        Console.WriteLine("Vads tika dzests!");
                        break;
                    case "3":
                        service.PropagateAll();
                        foreach (var gate in gates)
                        {
                            gate.Process();
                        }
                        break;
                    case "4":
                        var allWires = service.GetAll();
                        if (allWires.Count == 0)
                        {
                            Console.WriteLine("Nav izveidotu vadu!");
                            break;
                        }
                        Console.WriteLine("══════════════════════════════════");
                        Console.WriteLine("  VADU SARAKSTS");
                        Console.WriteLine("══════════════════════════════════");
                        Console.WriteLine($"{"ID",-6} {"No Gate",-10} {"Uz Gate",-10} {"Signāls"}");
                        Console.WriteLine(new string('─', 40));
                        foreach (var w in allWires)
                        {
                            Console.WriteLine($"{w.Id,-6} {w.StartGate.Id,-10} {w.EndGate.Id,-10} {(w.Data ? 1 : 0)}");
                        }
                        Console.WriteLine();
                        break;
                    case "5":
                        Console.WriteLine("══════════════════════════════════");
                        Console.WriteLine("  VĀRTI");
                        Console.WriteLine("══════════════════════════════════");
                        Console.WriteLine($"{"ID",-5} {"Tips",-8} {"Ieejas",-30} {"Izeja"}");
                        Console.WriteLine(new string('─', 54));
                        foreach (var gate in gates)
                        {
                            string inputs = string.Join(", ", gate.Inputs.Select((p, i) => $"P{i + 1}={(p.Data ? 1 : 0)}"));
                            Console.WriteLine($"{gate.Id,-5} {gate.Name,-8} {inputs,-30} {(gate.Output.Data ? 1 : 0)}");
                        }
                        Console.WriteLine();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Nepareiza izvēle!");
                        break;
                }
            }
        }



    }
    // ═══════════════════════════════════════════
    //  ENUM
    // ═══════════════════════════════════════════
    public enum GateType { And, Or, Not, Nand, Nor, Xor, Xnor }

    // ═══════════════════════════════════════════
    //  PORTI
    // ═══════════════════════════════════════════
    public class InputPort
    {
        public bool Data { get; set; }
    }

    public class OutputPort
    {
        public bool Data { get; set; }
    }

    // ═══════════════════════════════════════════
    //  GATE HIERARHIJA
    // ═══════════════════════════════════════════
    public abstract class Gate
    {
        private static int _idCounter = 1;

        public int Id { get; private set; }
        public string Name { get; protected set; }
        public GateType Type { get; protected set; }
        public List<InputPort> Inputs { get; set; } = new List<InputPort>();
        public OutputPort Output { get; set; } = new OutputPort();

        protected Gate() { Id = _idCounter++; }

        public void Process() { Output.Data = Calculate(); }
        protected abstract bool Calculate();
    }

    public class AndGate : Gate { public AndGate() { Type = GateType.And; Name = "AND"; } protected override bool Calculate() => Inputs.All(p => p.Data); }
    public class OrGate : Gate { public OrGate() { Type = GateType.Or; Name = "OR"; } protected override bool Calculate() => Inputs.Any(p => p.Data); }
    public class NotGate : Gate { public NotGate() { Type = GateType.Not; Name = "NOT"; } protected override bool Calculate() => Inputs.Count > 0 && !Inputs[0].Data; }
    public class NandGate : Gate { public NandGate() { Type = GateType.Nand; Name = "NAND"; } protected override bool Calculate() => !Inputs.All(p => p.Data); }
    public class XorGate : Gate { public XorGate() { Type = GateType.Xor; Name = "XOR"; } protected override bool Calculate() => Inputs.Count(p => p.Data) % 2 != 0; }


    //======================================================================================//
    //WireService nodrošina vadu izveidi, dzēšanu un datu propagāciju starp vārtiem.        //
    //Tā ir atbildīga par vadu savienošanu starp vārtiem, datu pārraidi un vadu pārvaldību. //
    //Šī klase ļauj lietotājam izveidot savienojumus starp vārtiem,                         //
    //Dzēst nevajadzīgus vadus un propagēt signālus caur visiem savienojumiem!              //
    //======================================================================================//

    // WireService Satur datus Start portā un Beigas portā ( StartPort un EndPort) 
    // un arī StartGate un EndGate, lai zinātu, kuri vārti ir savienoti ar vadu.
    // Tie veidos iespēju rediģēt un pārnest datus starp vārtiem, izmantojot vadus.
    public class WireService
    {
        private List<Wire> _wires = new List<Wire>();
        public List<Wire> GetAll() { return _wires; }
        //atgriež visus izveidotos vadus, lai varētu tos parādīt vai pārvaldīt.
        //Nepieciešams atgriezt vārtus lai ar to varētu strādāt Main klasē.

        public void Connect(Wire newwire, Gate foundgateO, InputPort portI, Gate foundgateI) 
        {
            

            newwire.StartPort = foundgateO.Output;  // savieno vārta Output portu ar vada StartPortu
            newwire.EndPort = portI;                // savieno vārta Input portu ar vada EndPortu
            newwire.StartGate = foundgateO;         // saglabā informāciju par vārtiem, kuri ir savienoti ar vadu
            newwire.EndGate = foundgateI;           // saglabā informāciju par vārtiem, kuri ir savienoti ar vadu
            _wires.Add(newwire);



        }
        public void Disconnect()
        {
            
            int wireid = int.Parse(Console.ReadLine());
            foreach ( var wire in _wires)
            {
                if (wire.Id == wireid)
                {
                    _wires.Remove(wire);
                  
                    return; // ja operacija ir veiksmīga, tad iziet no funkcijas,
                            // lai izvairītos no "Vads nav atrasts!" paziņojuma
                }
                Console.WriteLine("Vads nav atrasts! Nav ko dzest!");
            }
        }

        //PropagateAll funkcija nodrošina datu pārraidi caur visiem izveidotajiem vadiem.
        // Apstiprina visas izmaiņas vados un veido jaunus pārraides kanālus starp vārtiem.
        public void PropagateAll() 
        {
            foreach (var wire in _wires)
            {
                wire.EndPort.Data = wire.StartPort.Data;
            }
        }
    }
    public class Wire
    {
        private static int _idCounter = 1; // Statiskā mainīgā, kas tiek izmantota unikālu ID
                                           // piešķiršanai katram izveidotajam vadam
        public int Id { get; private set; } // Unikāls ID katram vadam

        public Wire() { Id = _idCounter++; } // Automātiski piešķir unikālu ID katram izveidotajam vadam
        public OutputPort StartPort { get; set; } // Vads sākas no vārta Output porta, tāpēc StartPort ir OutputPort tips
        public InputPort EndPort { get; set; }    // Vads beidzas vārta Input portā, tāpēc EndPort ir InputPort tips ( otrādi ) 
        public bool Data => StartPort?.Data ?? false; // Datu pārraide: vada signāls tiek noteikts pēc StartPort datiem.
                                                      // Ja StartPort nav savienots, tad datu vērtība ir false.
                                                      // Tas ļauj izvarīties no kļūdām, ja vads nav pilnībā savienots,
                                                      // un nodrošina, ka neizveidoti vadi neietekmē signālu pārraidi.

        public Gate StartGate { get; set; }  // Glābā informāciju par vārtiem, kuri ir savienoti ar vadu ( Sākums ) 
                                           
        public Gate EndGate { get; set; }    // Glābā informāciju par vārtiem, kuri ir savienoti ar vadu ( Beigas )





    }
}
