using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace newToC
{
    public class NFAMap {
        public List<int> acceptedStates = new List<int>();
        public List<char> Alphabet = new List<char>();
        public int startingState = new int();
        public int totalNumStates = 0;
        public List<NFAElement> AllStates = new List<NFAElement>(); 
    }

    public class NFAElement {
        public int state = -1;
        public List<NFAMove> possibleMoves = new List<NFAMove>();
    }

    public class NFAMove {
        public char letter = '!';
        public int endingState = -1;
    }

    public class DFAMap {
        public List<int> acceptedStates = new List<int>();
        public List<char> Alphabet = new List<char>();
        public int startingState = new int();
        public int totalNumStates = 0;
        public List<DFAElement> AllStates = new List<DFAElement>();
    }

    public class DFAElement {
        public List<int> states = new List<int>();
        public List<DFAMove> possibleMoves = new List<DFAMove>();
    }

    public class DFAMove {
        public char letter = '!';
        public List<int> endingStates = new List<int>();
    }



    public class Worker {
        NFAMap NFA;

        public void readFileAndCreateNFA() {
            System.IO.StreamReader file = new System.IO.StreamReader("Input.txt");
            string line = file.ReadLine();
            NFA = new NFAMap();
            NFA.totalNumStates = Int32.Parse(line); //Get the total number of states

            //Add the NFA elements to the allstates list
            for (int i = 0; i < NFA.totalNumStates; i++) {
                NFA.AllStates.Add(new NFAElement());
                NFA.AllStates[i].state = i;
            }

            line = file.ReadLine();
            foreach (char c in line)
                NFA.Alphabet.Add(c);    //Create Alphabet list

            line = file.ReadLine();
            for (int i = 0; i < line.Count(); i++)
                if (line[i].ToString() != " ")
                    NFA.acceptedStates.Add((int)(line[i] - '0')); //Create Final States list

            line = file.ReadLine();
            NFA.startingState = Int32.Parse(line); //Get initial state

            //Add the moves to each NFAelement
            while ((line = file.ReadLine()) != null) {
                NFAMove move = new NFAMove();
                int start = ((int)(line[0] - '0'));
                move.letter = line[2];
                move.endingState = ((int)(line[4] - '0'));

                //Find the matching element and add the move to it
                foreach (var element in NFA.AllStates) {
                    if (element.state == start) {
                        element.possibleMoves.Add(move);
                        break;
                    }
                }
            }
        }

        public void createDFA() {
            DFAMap DFA = new DFAMap();
            DFA.Alphabet = NFA.Alphabet; //use alphabet from NFA
            DFA.startingState = NFA.startingState; //use alphabet from NFA

            //Create first state in DFA
            DFAElement first= new DFAElement();
            DFAMove move = new DFAMove();
            first.states.Add(DFA.startingState);

            //Add each possible move
            foreach (var c in DFA.Alphabet) {
                move = new DFAMove();
                move.letter = c;
                first.possibleMoves.Add(move);
            }

            //Determine where the moves can end up
            foreach (var DFAelement in first.possibleMoves) { 
                foreach (var NFAelement in NFA.AllStates)
                    if (NFAelement.state == first.states[0])
                        foreach (var posMove in NFAelement.possibleMoves)
                            if (posMove.letter == DFAelement.letter)
                                DFAelement.endingStates.Add(posMove.endingState);
                    
            }
            DFA.AllStates.Add(first); //Add the first state into the DFA states

            CheckAndCreateDFAElement(DFA);

            foreach (var item in DFA.AllStates) {
                DFA.totalNumStates += 1;
            }
            DFA.acceptedStates = NFA.acceptedStates;
        }

        public void CheckAndCreateDFAElement(DFAMap DFA) {
            //Look for elements to add
            bool foundNewElemenet = false;
            DFAElement elementToAdd = new DFAElement();
            DFAElement dummy = new DFAElement();

            //Checks if the DFA element exists ...Sorry it's confusing
            foreach (var DFAelement in DFA.AllStates) {
                foreach (var moveOption in DFAelement.possibleMoves) {                       
                        bool check = false;
                        foreach (var dfaElement in DFA.AllStates) {
                            if (dfaElement.states.SequenceEqual(moveOption.endingStates)) {
                                check = true;
                                continue;
                            }
                        }
                        if (check == false) {
                            foundNewElemenet = true;
                            elementToAdd.states = moveOption.endingStates;
                            break;
                        }
                        check = false;
                }
                if (foundNewElemenet)
                    break;
            }

            //end the recursion when there are no more elements left to find
            if (!foundNewElemenet)
                return;

            //Element to add was found, so add it
            //Add each possible move
            DFAMove move;
            foreach (var c in DFA.Alphabet) {
                move = new DFAMove();
                move.letter = c;
                elementToAdd.possibleMoves.Add(move);
            }

            //Create new element
            foreach (var DFAelement in elementToAdd.possibleMoves) { //each move possibilty from first state
                foreach (var NFAelement in NFA.AllStates)
                    foreach (var state in elementToAdd.states)
                    if (NFAelement.state == state) 
                        foreach (var posMove in NFAelement.possibleMoves)
                            if (posMove.letter == DFAelement.letter)
                                DFAelement.endingStates.Add(posMove.endingState);
            }
            DFA.AllStates.Add(elementToAdd); //Add new element to the DFA list

            //Recursively check if there are more states to add and then add them
            CheckAndCreateDFAElement(DFA);
        }

        public void writeToFile() {


        }
    }

    public class Program
    {
        static void Main(string[] args) {
            Worker work = new Worker();
            work.readFileAndCreateNFA();
            work.createDFA();
            work.writeToFile();
        }
    }
}
