using System;
using System.Collections;

namespace WvS
{
    enum Level
    {
        easy = 1,
        hard,
        extream,
    }

    enum Direction
    {
        up,
        down,
        left,
        right,
        invalid,
    }

    enum MoveResult
    {
        can_move,
        can_not_move,
        no_sheep_to_eat,
    }

    struct Position
    {
        public int x;
        public int y;
    }

    class Unit
    {
        public string name;
        public string tag;
        public Position position;
        public Position Move(Direction direction, int moveStep)
        {
            switch(direction)
            {
                case Direction.up:
                    {
                        position.x -= moveStep;
                        break;
                    }
                case Direction.down:
                    {
                        position.x += moveStep;
                        break;
                    }
                case Direction.left:
                    {
                        position.y -= moveStep;
                        break;
                    }
                case Direction.right:
                    {
                        position.y += moveStep;
                        break;
                    }
            }
            return position;
        }
    }

    class Sheep : Unit { }

    class Wolf : Unit
    {
        public void Eat()
        {

        }
    }

    class Program
    {
        static void Init2DArray(string[,] array)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    array[i,j] = "     ";
                }
            }
        }

        static void UpdateMap(string[,] map, ArrayList sheeps, ArrayList wolfs)
        {
            Init2DArray(map);

            foreach (Sheep sheep in sheeps)
            {
                map[sheep.position.x, sheep.position.y] = sheep.name;
            }
            foreach (Wolf wolf in wolfs)
            {
                map[wolf.position.x, wolf.position.y] = wolf.name;
            }
        }

        static void PrintMap(string[,] map)
        {
            Console.WriteLine("=============================");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (j == 4)
                    {
                        Console.WriteLine(map[i, j]);
                    }
                    else
                    {
                        Console.Write(map[i, j] + "-");
                    }
                }
                if (i != 4)
                {
                    Console.WriteLine("|      |      |      |      |");
                }
            }
            Console.WriteLine("=============================");
        }

        static public void UnitsInit(ArrayList wolfs, ArrayList sheeps)
        {
            int offset = 0;
            foreach (Wolf wolf in wolfs)
            {
                wolf.name = "wolf"+(offset+1);
                wolf.tag = "wolf";
                wolf.position.x = 4;
                wolf.position.y = 2 * offset + 1;
                offset++;
            }

            offset = 0;
            foreach (Sheep sheep in sheeps)
            {
                sheep.name = "shep"+offset;
                sheep.tag = "sheep";
                sheep.position.x = offset/5;
                sheep.position.y = offset%5;
                offset++;
            }
        }

        static bool CheckUnitInGroup(ArrayList wolfs, string wolfName)
        {
            foreach(Wolf wolf in wolfs)
            {
                if(wolf.name == wolfName)
                {
                    return true;
                }
            }
            return false;
        }

        static bool GetPlayerCommand(ref string unitName, ref string action, ref string direction, ArrayList wolfs)
        {
            Console.WriteLine("Please input your order : ");
            string command = Console.ReadLine();
            string[] tempStr = command.Split(" ");
            if(tempStr.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("user command invalid!");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
            unitName = tempStr[0];
            if(!CheckUnitInGroup(wolfs, unitName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0} is no in wolf group!", unitName);
                Console.Write("The wolf group contain : ");
                foreach(Wolf wolf in wolfs)
                {
                    Console.Write(wolf.name + " ");
                }
                Console.WriteLine("");
                return false;
            }
            action = tempStr[1];
            if("move" != action && "eat" != action)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("The action should be 'move' or 'eat'");
                return false;
            }
            direction = tempStr[2];
            if(!Enum.IsDefined(typeof(Direction), direction))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("The direction should be 'up'/'down'/'left'/'right'!");
                return false;
            }
            return true;
        }

        static void PreDrawNextMap(string[,] nextMap, Sheep sheep, Position nextPos)
        {
            nextMap[sheep.position.x, sheep.position.y] = "     ";
            nextMap[nextPos.x, nextPos.y] = sheep.name;
        }

        static Direction JudgeSheepCanMove(Sheep sheep, string[,] map)
        {
            ArrayList directionArray = new ArrayList{ "up", "down", "right", "left" };
            string[,] nextMap = (string[,])map.Clone();
            Random rd = new Random();

            do
            {
                int index = rd.Next(0, directionArray.Count);
                string direction = (string)directionArray[index];
                Direction dir = (Direction)Enum.Parse(typeof(Direction), direction);
                switch(dir)
                {
                    case Direction.up:
                        {
                            if(sheep.position.x == 0 || map[sheep.position.x-1, sheep.position.y] != "     ")
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            Position nextPos = new Position
                            {
                                x = sheep.position.x - 1,
                                y = sheep.position.y
                            };
                            PreDrawNextMap(nextMap, sheep, nextPos);
                            if (!IsSheepSafe(nextPos, nextMap))
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            break;
                        }
                    case Direction.down:
                        {
                            if(sheep.position.x == 4 || map[sheep.position.x+1,sheep.position.y] != "     ")
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            Position nextPos = new Position
                            {
                                x = sheep.position.x + 1,
                                y = sheep.position.y
                            };
                            PreDrawNextMap(nextMap, sheep, nextPos);
                            if (!IsSheepSafe(nextPos, nextMap))
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            break;
                        }
                    case Direction.left:
                        {
                            if (sheep.position.y == 0 || map[sheep.position.x, sheep.position.y-1] != "     ")
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            Position nextPos = new Position
                            {
                                x = sheep.position.x,
                                y = sheep.position.y - 1
                            };
                            PreDrawNextMap(nextMap, sheep, nextPos);
                            if (!IsSheepSafe(nextPos, nextMap))
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            break;
                        }
                    case Direction.right:
                        {
                            if (sheep.position.y == 4 || map[sheep.position.x, sheep.position.y+1] != "     ")
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            Position nextPos = new Position
                            {
                                x = sheep.position.x,
                                y = sheep.position.y + 1
                            };
                            PreDrawNextMap(nextMap, sheep, nextPos);
                            if (!IsSheepSafe(nextPos, nextMap))
                            {
                                directionArray.RemoveAt(index);
                                continue;
                            }
                            break;
                        }
                }
                return dir;
            } while (0 != directionArray.Count);
            return Direction.invalid;
        }

        static MoveResult JudgeWolfCanMove(Wolf wolf, string action, Direction direction, string[,] map, int step)
        {
            if("eat"==action)
            {
                if(MoveResult.can_not_move == JudgeWolfCanMove(wolf, "move", direction, map, step - 1))
                {
                    return MoveResult.can_not_move;
                }
            }
            string destObjectName = ("eat" == action) ? "shep" : "     ";
            MoveResult errRes = ("eat" == action) ? MoveResult.no_sheep_to_eat : MoveResult.can_not_move;
            switch (direction)
            {
                case Direction.up:
                    {
                        if(wolf.position.x <= step-1 || !map[wolf.position.x-step, wolf.position.y].Contains(destObjectName))
                        {
                            return errRes;
                        }
                        break;
                    }
                case Direction.down:
                    {
                        if (wolf.position.x >= 5-step || !map[wolf.position.x+step, wolf.position.y].Contains(destObjectName))
                        {
                            return errRes;
                        }
                        break;
                    }
                case Direction.left:
                    {
                        if (wolf.position.y <= step-1 || !map[wolf.position.x, wolf.position.y-step].Contains(destObjectName))
                        {
                            return errRes;
                        }
                        break;
                    }
                case Direction.right:
                    {
                        if (wolf.position.y >= 5-step || !map[wolf.position.x, wolf.position.y+step].Contains(destObjectName))
                        {
                            return errRes;
                        }
                        break;
                    }
                default:
                    {
                        return errRes;
                    }
            }
            return MoveResult.can_move;
        }

        static public void KillSheep(ArrayList sheeps, string sheepName)
        {
            foreach(Sheep sheep in sheeps)
            {
                if(sheep.name == sheepName)
                {
                    sheeps.Remove(sheep);
                    break;
                }
            }
        }

        static public string GetKilledSheepNameByPos(ArrayList sheeps, Position sheepPos)
        {
            foreach(Sheep sheep in sheeps)
            {
                if(sheep.position.x == sheepPos.x && sheep.position.y == sheepPos.y)
                {
                    return sheep.name;
                }
            }
            return "";
        }

        static bool MoveWolf(string wolfName, ArrayList wolfs, string action, Direction direction, string[,] map, ArrayList sheeps)
        {
            int moveStep = ("eat" == action) ? 2 : 1;
            foreach(Wolf wolf in wolfs)
            {
                if(wolf.name == wolfName)
                {
                    MoveResult res = JudgeWolfCanMove(wolf, action, direction, map, moveStep);
                    if (MoveResult.can_move != res)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (MoveResult.can_not_move == res)
                        {
                            Console.WriteLine("{0} can't move {1}!", wolfName, direction);
                        }
                        else
                        {
                            Console.WriteLine("{0} can't eat sheep because {1} has no sheep !", wolfName, direction);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        return false;
                    }
                    Position killedSheepPos = wolf.Move(direction, moveStep);
                    if("eat" == action)
                    {
                        string killedSheepName = GetKilledSheepNameByPos(sheeps, killedSheepPos);
                        KillSheep(sheeps, killedSheepName);
                    }
                    UpdateMap(map, sheeps, wolfs);
                    return true;
                }
            }
            return false;
        }

        static bool CheckUp(Position sheepPos, string[,] map)
        {
            if (sheepPos.x <= 1)
            {
                return true;
            }
            else if(map[sheepPos.x-1,sheepPos.y] !=  "     ")
            {
                return true;
            }
            else if(map[sheepPos.x-2, sheepPos.y].Contains("wolf"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool CheckDown(Position sheepPos, string[,] map)
        {
            if (sheepPos.x >= 3)
            {
                return true;
            }
            else if (map[sheepPos.x + 1, sheepPos.y] != "     ")
            {
                return true;
            }
            else if (map[sheepPos.x + 2, sheepPos.y].Contains("wolf"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool CheckLeft(Position sheepPos, string[,] map)
        {
            if (sheepPos.y <= 1)
            {
                return true;
            }
            else if (map[sheepPos.x, sheepPos.y-1] != "     ")
            {
                return true;
            }
            else if (map[sheepPos.x, sheepPos.y-2].Contains("wolf"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool CheckRight(Position sheepPos, string[,] map)
        {
            if (sheepPos.y >= 3)
            {
                return true;
            }
            else if (map[sheepPos.x, sheepPos.y+1] != "     ")
            {
                return true;
            }
            else if (map[sheepPos.x, sheepPos.y+2].Contains("wolf"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool IsSheepSafe(Position sheepPos, string[,] map)
        {
            return CheckUp(sheepPos, map) &&
                   CheckDown(sheepPos, map) &&
                   CheckLeft(sheepPos, map) &&
                   CheckRight(sheepPos, map);
        }

        static void CheckSheepSafety(ArrayList sheeps, ArrayList targetSheeps, ArrayList safeSheeps, string[,] map)
        {
            foreach(Sheep sheep in sheeps)
            {
                if(IsSheepSafe(sheep.position, map))
                {
                    safeSheeps.Add(sheep);
                }
                else
                {
                    targetSheeps.Add(sheep);
                }
            }
        }

        static void RandomSheepMove(string[,] map, ArrayList sheeps, ref bool moved)
        {
            Random rd = new Random();
            int sheepIndex = rd.Next(0, sheeps.Count);
            Sheep sheep = (Sheep)sheeps[sheepIndex];
            Direction moveDir = JudgeSheepCanMove(sheep, map);
            if (Direction.invalid != moveDir)
            {
                sheep.Move(moveDir, 1);
                moved = true;
                return;
            }
        }

        static void EasyProc(string[,] map, ArrayList targetSheeps, ArrayList safeSheeps, ref bool moved)
        {
            while (!moved && targetSheeps.Count > 0)
            {
                RandomSheepMove(map, targetSheeps, ref moved);
                if (!moved && targetSheeps.Count == 1) break;
            }

            while (!moved && safeSheeps.Count > 0)
            {
                RandomSheepMove(map, safeSheeps, ref moved);
                if (!moved && safeSheeps.Count == 1) break;
            }
        }

        static int CalcDangerSheepNum(string[,] nextMap)
        {
            int dangerSheepNum = 0;
            for (int xLoop = 0; xLoop < 5; xLoop++)
            {
                for (int yLoop = 0; yLoop < 5; yLoop++)
                {
                    if(nextMap[xLoop,yLoop].Contains("shep"))
                    {
                        Position sP = new Position
                        {
                            x = xLoop,
                            y = yLoop,
                        };
                        if (!IsSheepSafe(sP, nextMap)) dangerSheepNum++; 
                    }
                }
            }
            return dangerSheepNum;
        }

        static void SheepChoosesBestMovement(Sheep sheep, string[,] map, ref int dangerSheepNum, ref string bestMove, int curDangerSheepNUm)
        {
            ArrayList directionArray = new ArrayList { "up", "down", "right", "left" };

            int minDangerNum = curDangerSheepNUm;
            int calcDangerNum = 0;
            ArrayList moveGroup = new ArrayList();
            for (int dirIndex = 0; dirIndex < directionArray.Count; dirIndex++)
            {
                string[,] nextMap = (string[,])map.Clone();
                Position nextPos = new Position();
                string direction = (string)directionArray[dirIndex];
                Direction dir = (Direction)Enum.Parse(typeof(Direction), direction);
                switch (dir)
                {
                    case Direction.up:
                        {
                            if (sheep.position.x == 0 || map[sheep.position.x - 1, sheep.position.y] != "     ")
                            {
                                continue;
                            }

                            nextPos.x = sheep.position.x - 1;
                            nextPos.y = sheep.position.y;
                            break;
                        }
                    case Direction.down:
                        {
                            if (sheep.position.x == 4 || map[sheep.position.x + 1, sheep.position.y] != "     ")
                            {
                                continue;
                            }

                            nextPos.x = sheep.position.x + 1;
                            nextPos.y = sheep.position.y;
                            break;
                        }
                    case Direction.left:
                        {
                            if (sheep.position.y == 0 || map[sheep.position.x, sheep.position.y - 1] != "     ")
                            {
                                continue;
                            }
                            nextPos.x = sheep.position.x;
                            nextPos.y = sheep.position.y - 1;
                            break;
                        }
                    case Direction.right:
                        {
                            if (sheep.position.y == 4 || map[sheep.position.x, sheep.position.y + 1] != "     ")
                            {
                                continue;
                            }
                            nextPos.x = sheep.position.x;
                            nextPos.y = sheep.position.y + 1;
                            break;
                        }
                }

                PreDrawNextMap(nextMap, sheep, nextPos);
                calcDangerNum = CalcDangerSheepNum(nextMap);
                if (calcDangerNum < minDangerNum)
                {
                    moveGroup.Clear();
                    moveGroup.Add(direction);
                    minDangerNum = calcDangerNum;
                }
                else if (calcDangerNum == minDangerNum)
                {
                    moveGroup.Add(direction);
                }
            }

            dangerSheepNum = minDangerNum;
            if (moveGroup.Count > 0)
            {
                Random rd = new Random();
                int moveIndex = rd.Next(0, moveGroup.Count);
                bestMove = (string)moveGroup[moveIndex];
            }
            else
            {
                bestMove = "invalid";
            }
        }

        static void HardProc(string[,] map, ArrayList sheeps, int dangerSheepNum, ref bool moved)
        {
            int[] dangerSheepNumAfterMove = new int[sheeps.Count];
            string[] bestMovementPerSheep = new string[sheeps.Count];
            for (int sheepLoop = 0; sheepLoop < sheeps.Count; sheepLoop++)
            {
                Sheep sheep = (Sheep)sheeps[sheepLoop];
                SheepChoosesBestMovement(sheep, map, ref dangerSheepNumAfterMove[sheepLoop], ref bestMovementPerSheep[sheepLoop], dangerSheepNum);
            }

            int minDangerSheepNum = dangerSheepNum;
            ArrayList moveSheepGroup = new ArrayList();
            for (int arrLoop = 0; arrLoop < dangerSheepNumAfterMove.Length; arrLoop++)
            {
                if(dangerSheepNumAfterMove[arrLoop] < minDangerSheepNum)
                {
                    moveSheepGroup.Clear();
                    minDangerSheepNum = dangerSheepNumAfterMove[arrLoop];
                    moveSheepGroup.Add(arrLoop);
                }
                else if(dangerSheepNumAfterMove[arrLoop] == minDangerSheepNum)
                {
                    moveSheepGroup.Add(arrLoop);
                }
            }

            if(moveSheepGroup.Count > 1)//todo 选择错误
            {
                Random rd = new Random();
                int index = rd.Next(0, moveSheepGroup.Count);
                int moveSheepIndex = (int)moveSheepGroup[index];
                if ("invalid" != bestMovementPerSheep[moveSheepIndex])
                {
                    Sheep sheep = (Sheep)sheeps[moveSheepIndex];
                    Direction moveDir = (Direction)Enum.Parse(typeof(Direction), bestMovementPerSheep[moveSheepIndex]);
                    sheep.Move(moveDir, 1);
                    moved = true;
                }
            }
            else if(moveSheepGroup.Count == 1)
            {
                int moveSheepIndex = (int)moveSheepGroup[0];
                if ("invalid" != bestMovementPerSheep[moveSheepIndex])
                {
                    Sheep sheep = (Sheep)sheeps[moveSheepIndex];
                    Direction moveDir = (Direction)Enum.Parse(typeof(Direction), bestMovementPerSheep[moveSheepIndex]);
                    sheep.Move(moveDir, 1);
                    moved = true;
                }
            }

            return;
        }

        static void ExtreamProc(string[,] map, ArrayList targetSheeps, ArrayList safeSheeps, ref bool moved)
        {
        }

        static bool MoveSheep(ArrayList sheeps, string[,] map, Level gameLv)
        {
            if (0 == sheeps.Count)
            {
                return false;
            }

            ArrayList targetSheeps = new ArrayList();
            ArrayList safeSheeps = new ArrayList();

            CheckSheepSafety(sheeps, targetSheeps, safeSheeps, map);

            bool moved = false;
            switch(gameLv)
            {
                case Level.easy:
                    EasyProc(map, targetSheeps, safeSheeps, ref moved);
                    break;
                case Level.hard:
                    HardProc(map, sheeps, targetSheeps.Count, ref moved);
                    break;
                case Level.extream:
                    ExtreamProc(map, targetSheeps, safeSheeps, ref moved);
                    break;
            }

            return moved;
        }

        static Level ChooseGameLevel()
        {
            Console.WriteLine("Please select the game level by enter the number:");
            Console.WriteLine("1 : Easy");
            Console.WriteLine("2 : Hard");
            Console.WriteLine("3 : Extream");
            int level = int.Parse(Console.ReadLine());
            while (!Enum.IsDefined(typeof(Level), level))
            {
                Console.WriteLine("Invalid input! Please input again.");
                level = int.Parse(Console.ReadLine());
            }
            Console.WriteLine("You select the '{0}' level.", Enum.GetName(typeof(Level), level));
            return (Level)level;
        }

        static void Main(string[] args)
        {
            string unitName = "";
            string action = "";
            string direction = "";
            string[,] map = new string[5, 5];
            Program.Init2DArray(map);

            Sheep sheep1 = new Sheep();
            Sheep sheep2 = new Sheep();
            Sheep sheep3 = new Sheep();
            Sheep sheep4 = new Sheep();
            Sheep sheep5 = new Sheep();
            Sheep sheep6 = new Sheep();
            Sheep sheep7 = new Sheep();
            Sheep sheep8 = new Sheep();
            Sheep sheep9 = new Sheep();
            Sheep sheep10 = new Sheep();
            Wolf wolf1 = new Wolf();
            Wolf wolf2 = new Wolf();

            ArrayList sheeps = new ArrayList {sheep1, sheep2, sheep3, sheep4, sheep5,
                                              sheep6, sheep7, sheep8, sheep9, sheep10};
            ArrayList wolfs = new ArrayList{wolf1, wolf2};
            UnitsInit(wolfs, sheeps);

            Level lv = ChooseGameLevel();
            do
            {
                UpdateMap(map, sheeps, wolfs);
                PrintMap(map);
                if(!GetPlayerCommand(ref unitName, ref action, ref direction, wolfs))
                {
                    continue;
                }
                Direction dir = (Direction)Enum.Parse(typeof(Direction), direction);
                if (!MoveWolf(unitName, wolfs, action, dir, map, sheeps))
                {
                    continue;
                }
                if (!MoveSheep(sheeps, map, lv)) 
                {
                    UpdateMap(map, sheeps, wolfs);
                    PrintMap(map);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("All sheeps have been eatten!");
                    Console.WriteLine("You Win !");
                    break;
                }
            } while (true);
        }
    }
}
