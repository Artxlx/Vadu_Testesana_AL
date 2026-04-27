using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vadu_Testesana_AL
{
    internal class Program
    {
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

            // ŠEIT PIEVIENO SAVU KODU PAR VADIEM
            // Piemēram:
            //   var wireService = new WireService();
            //   wireService.Connect(gateA.Output, gateB.Inputs[0]);
            //   wireService.PropagateAll();
            //   ... izvade ...

            Console.WriteLine("(vadu kods vel nav pievienots)");
            Console.WriteLine();



            bool running = true;
            while (running)
            {
                Console.WriteLine("══════════════════════════════════");
                Console.WriteLine("  VADU IZVĒLNE");
                Console.WriteLine("══════════════════════════════════");
                Console.WriteLine("1. Savienot vadu");
                Console.WriteLine("2. Atvienot vadu");
                Console.WriteLine("3. Propagēt visus vadus");
                Console.WriteLine("4. Parādīt visus vadus");
                Console.WriteLine("0. Iziet");
                Console.WriteLine("══════════════════════════════════");
                Console.Write("Izvēle: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Wire newwire = new Wire();
                        WireService service = new WireService();
                        Console.Write("Kuram Gate output portam izveidot sakuma savienojumu. "); 
                        int gateidO = int.Parse(Console.ReadLine());

                        Gate foundgateO = gates.FirstOrDefault(g => g.Id == gateidO);

                        if (foundgateO == null)
                        {
                            Console.WriteLine("Vārti nav atrasti!");
                            return;
                        }


                        Console.Write("Kuram gate input portam izveidot sakuma savienojumu. ");
                        int gateidI = int.Parse(Console.ReadLine());
                        Gate foundgateI = gates.FirstOrDefault(g => g.Id == gateidI);

                        if (foundgateI == null || foundgateO == foundgateI)
                        {
                            Console.WriteLine("Vārti nav atrasti vai ir vienādi!");
                            return;
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
                            return;
                        }
                        InputPort portI = foundgateI.Inputs[portidI];
                        


                        service.Connect(newwire, foundgateO, portI );
                        break;
                    case "2": // Disconnect
                        break;
                    case "3": // PropagateAll
                        break;
                    case "4": // DisplayAll
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
    

    public class WireContainer()
    {
        

    }

    public class WireService
    {
        private List<Wire> _wires = new List<Wire>();
        public void Connect(Wire newwire, Gate foundgateO, InputPort portI)
        {
            

            newwire.StartPort = foundgateO.Output;
            newwire.EndPort = portI;
            _wires.Add(newwire);
        }
        public void Disconnect()
        {
            Console.WriteLine("wire ID        StartPort      EndPort");
            for (int i = 0; i < _wires.Count; i++)
            {
                Wire wire = _wires[i];
                Console.Write("     " + wire.Id);
                Console.Write("     " + (wire.StartPort.Data ? 1 : 0));
                Console.Write("     " + (wire.EndPort.Data ? 1 : 0));
                Console.WriteLine();
            }
                Console.Write("Kuru vadu dzest:  ");
            int wireid = int.Parse(Console.ReadLine());
            foreach ( var wire in _wires)
            {
                if (wire.Id == wireid)
                {
                    _wires.Remove(wire);
                    Console.WriteLine("Vads tika dzests!");
                    return;
                }
            }
        }
        public void Propagade()
        {

        }
    }
    public class Wire
    {
        private static int _idCounter = 1;
        public int Id { get; private set; }

        public Wire() { Id = _idCounter++; }
        public OutputPort StartPort { get; set; }  // чей Output
        public InputPort EndPort { get; set; }     // чей Input
        public bool Data => StartPort?.Data ?? false; // берётся из StartPort

        





    }
}
