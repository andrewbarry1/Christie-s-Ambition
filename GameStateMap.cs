// The entire game ;)

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TheGame
{
    internal class GameStateMap : GameState
    {

        private ContentManager Content;
        private Game1 game;
        private County[] counties;
        private County individualCounty;
        private InputHandler inputHandler;

        private TextWindow icTextWindow;
        private TextWindow revWindow;

        private int week;

        private int funds;
        private int unionFunds;
        private int revenue;
        private int unionRevenue;
        private int ownedCounties;


        private bool beginning;
        private TextWindow beginningTextWindow;
        private Menu beginningMenu;
        private int beginningSelector;

        private int selectingForAttack;
        private int selectingForTroopTransfer;
        private bool selectingForRecruitment;

        private bool multiPlayer;
        private int playerNumber;
        private int enemyNumber;

        private Menu okayButton;

        private County targetCounty;
        private County sourceCounty;

        private Menu actionMenu; // general action menu
        private TextWindow gpWindow; // general-purpose Window

        public string[] countyNames;
        public int[] countyRevenues;
        public int[] countyPopulations;
        public int[][] countyGraph;
        public string[] playerNames;

        public Vector2[] labelOffsets;
        public TextWindow[] troopLabels;

        public GameStateMap(ContentManager c, Game1 g, int nP)
        {
            Content = c;
            game = g;

            week = 0;
            revenue = 0;
            unionRevenue = 0;
            funds = 0;
            unionFunds = 0;
            ownedCounties = 1;

            multiPlayer = (nP == 2);
            playerNumber = 1;
            enemyNumber = 2;

            selectingForAttack = 0;
            selectingForTroopTransfer = 0;
            selectingForRecruitment = false;


            actionMenu = null;
            gpWindow = null;

            playerNames = new string[] { "\\0UNOCCUPIED", "\\3CHRISTIE\\0", "\\4UNION\\0" };

            beginning = true;
            beginningSelector = 1;
            beginningTextWindow = new TextWindow(new String[] { playerNames[1] + " SELECT A COUNTY" },
                Content, new Vector2(25, 25), new Vector2(500, 25));


            inputHandler = new InputHandler();

            countyNames = new string[] {
                "Atlantic", "Bergen", "Burlington", "Camden", "Cape May", "Cumberland", "Essex",
                "Gloucester", "Hudson", "Hunterdon", "Mercer", "Middlesex", "Monmouth","Morris",
                "Ocean", "Passaic", "Salem", "Somerset", "Sussex", "Union", "Warren"};
            countyRevenues = new int[]
            { 27,42,34,29,33,21,31,31,31,48,36,33,40,47,29,26,27,47,35,34,32 };
            countyPopulations = new int[]
            { 274,905,448,513,97,156,783,288,634,128,366,809,630,492,576,501,66,323,149,536,108 };

            countyGraph = new int[][]
            {
                new int[] {2,3,4,5,7}, // Atlantic
                new int[] {6,8,15}, // Bergen
                new int[] {10,12,0,3,14}, // Burlington
                new int[] {0,2,7}, // Camden
                new int[] {0,5}, // Cape May
                new int[] {4,0,16,7}, // Cumberland
                new int[] {1,8,15,19,13}, // Essex
                new int[] {16,3,5,0}, // Gloucester
                new int[] {1,6}, // Hudson
                new int[] {20,13,17,10}, // Hunterdon
                new int[] {9,17,11,12,2}, // Mercer
                new int[] {19,17,10,12}, // Middlesex
                new int[] {10,11,2,14}, // Monmouth
                new int[] {18,15,6,19,17,9,20}, // Morris
                new int[] {12,2}, // Ocean
                new int[] {13,6,1,18}, // Passaic
                new int[] {7,5}, // Salem
                new int[] {13, 19, 11, 10, 9}, // Somerset
                new int[] {15,13,20}, // Sussex
                new int[] {6,13,17,11}, // Union
                new int[] {18,13,9} // Warren
            };

            labelOffsets = new Vector2[]
            {
                new Vector2(-30,0), // Atlantic
                new Vector2(-10,-10), // Bergen
                new Vector2(-15,-20), // Burlington
                new Vector2(-120, -70), // Camden
                new Vector2(30, 0), // Cape May
                new Vector2(0,0), // Cumberland
                new Vector2(-23, -20), // Essex
                new Vector2(-30, -20), // Gloucester
                new Vector2(10, -10), // Hudson
                new Vector2(-20, -20), // Hunterdon
                new Vector2(-30, -10), // Mercer
                new Vector2(-40, 0), // Middlesex
                new Vector2(0, -5), // Monmouth
                new Vector2(-20, -5), // Morris
                new Vector2(-40, -40), // Ocean
                new Vector2(0, -70), // Passaic
                new Vector2(-40, -35), // Salem
                new Vector2(-30, 0), // Somerset
                new Vector2(-40, -20), // Sussex
                new Vector2(30, 5), // Union
                new Vector2(-100, -10) // Warren
            };

            counties = new County[countyNames.Length];
            individualCounty = null;
            icTextWindow = null;

            int x = 0;
            foreach (string co in countyNames) // load county sprites
            {
                Sprite s = new Sprite(co + ".png", Content, new Vector2(650, 600));
                s.position = new Vector2(650 - s.boundingRectangle.Width, 600 - s.boundingRectangle.Height);
                Sprite sfull = new Sprite(co + "-full.png", Content, new Vector2(0, 0));
                counties[x] = new County(co, s, sfull, countyRevenues[x], countyPopulations[x], countyGraph[x++]);
            }

            troopLabels = new TextWindow[21];
            updateGameLabels();

        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (County c in counties)
            {
                c.drawMap(spriteBatch);
            }
            foreach (TextWindow tw in troopLabels)
            {
                tw.draw(spriteBatch);
            }
            if (individualCounty != null)
            {
                individualCounty.drawIndividual(spriteBatch);
                icTextWindow.draw(spriteBatch);
            }

            if (beginningTextWindow != null)
            {
                beginningTextWindow.draw(spriteBatch);
            }
            if (beginningMenu != null)
            {
                beginningMenu.draw(spriteBatch);
            }

            if (gpWindow != null)
            {
                gpWindow.draw(spriteBatch);
            }
            if (actionMenu != null)
            {
                actionMenu.draw(spriteBatch);
            }

            if (okayButton != null)
            {
                okayButton.draw(spriteBatch);
            }

            if (revWindow != null)
            {
                revWindow.draw(spriteBatch);
            }

        }

        public void doVictory(int winnerNumber)
        {
            actionMenu = null;
            string winnerString = "SOMEONE HAS WON"; // lol
            if (winnerNumber == 1)
            {
                winnerString = "\\2C\\1H\\2R\\1I\\2S\\1T\\2I\\1E \\2I\\1S \\2V\\1I\\2C\\1T\\2O\\1R\\2I\\1O\\2U\\1S\\n\\0NOW RUN FOR PRESIDENT";
            }
            else
            {
                winnerString = "\\2U\\1N\\2I\\1O\\2N\\1S \\2A\\1R\\2E \\1V\\2I\\1C\\2T\\1O\\2R\\1I\\2O\\1U\\2S\\n\\0CHRISTIE REMOVED FROM OFFICE";
            }
            gpWindow = new TextWindow(new string[] { winnerString },
                Content, new Vector2(25, 25), new Vector2(600, 50));
            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { returnToMainMenu }, 1, 1, "test-semifont.png", Content);
        }

        public void doDefeat()
        {
            gpWindow = new TextWindow(new string[] { "YOU WERE DEFEATED BY UNIONS\\nBETTER LUCK NEXT TIME" },
                Content, new Vector2(25, 25), new Vector2(600, 50));
            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { returnToMainMenu }, 1, 1, "test-semifont.png", Content);
        } // defeat message for Singleplayer

        public void takeCounty(County c, int ownership)
        {
            if (c.occupied == 1)
            {
                revenue -= c.revenue;
            }
            else if (c.occupied == 2)
            {
                unionRevenue -= c.revenue;
            }
            c.occupied = ownership;
            if (ownership == 1)
            {
                revenue += c.revenue;
            }
            else if (ownership == 2)
            {
                unionRevenue += c.revenue;
            }
        }

        public void returnToMainMenu()
        {
            game.removeState();
        }

        public void selectStartingCounty()
        {
            beginningMenu = null;
            if (individualCounty.occupied == 0)
            {
                takeCounty(individualCounty, beginningSelector);
            }
            else
            {
                cancelStartingCounty();
                return;
            }

            if (multiPlayer && beginningSelector != 2)
            {
                beginningSelector = 2;
                beginningTextWindow = new TextWindow(new String[] { playerNames[2] + " SELECT A COUNTY" },
                    Content, new Vector2(25, 25), new Vector2(500, 25));
            }
            else if (multiPlayer && beginningSelector == 2)
            {
                playerNumber = 1;
                beginning = false;
                beginningTextWindow = null;
                beginningMenu = null;
                individualCounty = null;
                icTextWindow = null;
                startWeek();
            }
            else
            {
                beginning = false;
                beginningTextWindow = null;
                beginningMenu = null;
                individualCounty = null;
                icTextWindow = null;
                int oCo = 0;
                while (oCo != 1)
                {
                    Random ra = new Random();
                    int toFill = ra.Next(0, counties.Length);
                    if (counties[toFill].occupied == 0)
                    {
                        takeCounty(counties[toFill], 2);
                        counties[toFill].occupied = 2;
                        oCo++;
                    }
                }
                startWeek();
            }

            return;
        }

        public void selectAttackCounty()
        {
            actionMenu = null;
            gpWindow = new TextWindow(new string[] { "SELECT TARGET" }, Content, new Vector2(25,25), new Vector2(250,25));
            selectingForAttack = 1;
        }

        public void startTroopMove()
        {
            actionMenu = null;
            gpWindow = new TextWindow(new string[] { "SELECT RECIPIENT" }, Content, new Vector2(25, 25), new Vector2(300, 25));
            selectingForTroopTransfer = 1;
        }

        public void doEnemyTurn()
        {
            Console.WriteLine("Starting enemy turn with " + unionFunds + " funds");
            actionMenu = null;
            List<County> enemyCounties = new List<County>();
            List<County> endangeredCounties = new List<County>();
            for (int x = 0; x < 21; x++)
            {
                if (counties[x].occupied == 2) enemyCounties.Add(counties[x]);
            }
            List<County> emptyAdjacencies = new List<County>();
            for (int x = 0; x < enemyCounties.Count; x++) // build relevant lists
            {
                int[] edges = enemyCounties[x].edges;
                for (int y = 0; y < edges.Length; y++)
                {
                    County dest = counties[edges[y]];
                    if (dest.occupied == 0)
                    {
                        emptyAdjacencies.Add(dest);
                    }
                    else if (dest.occupied == 1 && !endangeredCounties.Contains(counties[Array.IndexOf(counties, enemyCounties[x])]))
                    {
                        Console.WriteLine(counties[Array.IndexOf(counties, enemyCounties[x])].name + " is endangered");
                        endangeredCounties.Add(counties[Array.IndexOf(counties, enemyCounties[x])]);
                    }
                }
            }
            int[] sortedEmpty = new int[emptyAdjacencies.Count];
            for (int x = 0; x < emptyAdjacencies.Count; x++) // do intelligent expansion
            {
                sortedEmpty[x] = emptyAdjacencies[x].value;
            }
            int[] unsortedEmpty = (int[])sortedEmpty.Clone();
            Array.Sort(sortedEmpty);
            Array.Reverse(sortedEmpty);
            County[] emptyCountyArray = emptyAdjacencies.ToArray();
            Console.WriteLine("Empty Adjacencies: " + emptyCountyArray.Length);
            for (int x = 0; x < sortedEmpty.Length; x++)
            {
                int index = Array.IndexOf(unsortedEmpty, sortedEmpty[x]);
                County valuableCounty = emptyAdjacencies[index];
                County lowestBidder = null;
                int lbTroops = 1000000;
                for (int y = 0; y < valuableCounty.edges.Length; y++)
                {
                    County bidder = counties[valuableCounty.edges[y]];
                    if (bidder.troops < lbTroops && bidder.occupied == 2)
                    {
                        lowestBidder = bidder;
                        lbTroops = bidder.troops;
                    }
                }
                Console.WriteLine("Lowest Bidder for " + valuableCounty.name + ": " + lowestBidder.name);
                if (lowestBidder.troops / 2 < unionFunds)
                {
                    Console.WriteLine("Taking " + valuableCounty.name);
                    takeCounty(valuableCounty, 2);
                    unionFunds -= lowestBidder.troops / 2;
                }
            }
            Console.WriteLine("Dones expanding. Funds = " + unionFunds);
            Console.WriteLine("Beginning troop reallocation. Front line counties: " + endangeredCounties.Count);
            for (int x = 0; x < endangeredCounties.Count; x++) // reallocate troops and attack
            {
                County borderCounty = endangeredCounties[x];
                List<County> playerAdjacencies = new List<County>();
                County weakestAdjacent = null;
                int waTroops = 1000000;
                int[] edges = borderCounty.edges;
                for (int y = 0; y < edges.Length; y++)
                {
                    County adjacency = counties[edges[y]];
                    Console.WriteLine("Adjacency of " + borderCounty.name + ": " + adjacency.name);
                    Console.WriteLine("Adjacency troops: " + adjacency.troops);
                    Console.WriteLine("Occupation: " + adjacency.occupied);
                    if (adjacency.occupied == 1)
                    {
                        playerAdjacencies.Add(adjacency);
                        if (adjacency.troops < waTroops)
                        {
                            weakestAdjacent = adjacency;
                            waTroops = weakestAdjacent.troops;
                        }
                    }
                }
                if (weakestAdjacent == null)
                {
                    Console.WriteLine("Adjacent enemy tile has been eliminated this turn - moving on.");
                    continue;
                }
                Console.WriteLine("Weakest adjacency of " + borderCounty.name + " is " + weakestAdjacent.name);
                bool willAttack = false;
                int necessaryBuff = 0;
                if (borderCounty.troops > waTroops - 100 && unionFunds > borderCounty.troops/2)
                {
                    willAttack = true;
                    if (borderCounty.troops < waTroops * 1.3)
                    {
                        necessaryBuff = (int)(waTroops * 1.3) - borderCounty.troops;
                        Console.WriteLine("Necessary buff is " + necessaryBuff);
                    }
                }
                else
                {
                    necessaryBuff = waTroops - borderCounty.troops;
                    Console.WriteLine("Severely lacking - necessary buff is " + necessaryBuff + " and will not attack. Or too poor");
                }
                int currentBuff = 0;
                while (currentBuff < necessaryBuff) // find way to allocate troops to this area
                {
                    List<County> visited = new List<County>();
                    Stack<County> DFSstack = new Stack<County>();
                    DFSstack.Push(borderCounty);
                    while (DFSstack.Count != 0)
                    {
                        County chk = DFSstack.Pop();
                        if (visited.Contains(chk))
                        {
                            continue;
                        }
                        for (int y = 0; y < chk.edges.Length; y++)
                        {
                            if (counties[chk.edges[y]].occupied == 2 &&
                                !endangeredCounties.Contains(counties[chk.edges[y]]) && counties[chk.edges[y]] != borderCounty)
                            {
                                DFSstack.Push(counties[chk.edges[y]]);
                            }
                        }
                        if (chk != borderCounty)
                        {
                            visited.Add(chk);
                        }
                    }
                    if (visited.Count > 0)
                    {
                        County topCounty = visited[0];
                        for (int y = 0; y < visited.Count; y++)
                        {
                            if (visited[y].troops > topCounty.troops)
                            {
                                topCounty = visited[y];
                            }
                        }
                        Console.WriteLine("Strongest connected county to " + borderCounty.name + " is " + topCounty.name);
                        if ((topCounty.troops / 3) > 120 && unionFunds >= 90)
                        {
                            Console.WriteLine("moving troops from " + topCounty.name + " to " + borderCounty.name);
                            currentBuff += topCounty.troops / 3;
                            borderCounty.troops += (topCounty.troops / 3);
                            topCounty.troops -= (topCounty.troops / 3);
                            unionFunds -= 90;
                        }
                        else
                        {
                            Console.WriteLine("Too poor to move troops from there or it isnt worth it, going to recruit");
                            if (unionFunds >= 30)
                            {
                                int rNum = new Random().Next(30, 50);
                                borderCounty.troops += rNum;
                                currentBuff += rNum;
                                unionFunds -= 30;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("I'm all alone - recruit.");
                        if (unionFunds >= 30)
                        {
                            int rNum = new Random().Next(30, 50);
                            borderCounty.troops += rNum;
                            currentBuff += rNum;
                            unionFunds -= 30;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Console.WriteLine("Added " + currentBuff + " to troop count");
                if (currentBuff >= necessaryBuff && willAttack && unionFunds > borderCounty.troops / 2)
                {
                    Console.WriteLine("Doing battle against " + weakestAdjacent.name);
                    funds -= borderCounty.troops / 2;
                    Random ra = new Random();
                    int sourceOdds = 50 * (borderCounty.troops / weakestAdjacent.troops);
                    int targetOdds = 50 * (weakestAdjacent.troops / borderCounty.troops);
                    for (int y = 0; y < 500; y++)
                    {
                        int sourceCheck = ra.Next(0, 100);
                        if (sourceCheck > sourceOdds)
                        {
                            borderCounty.troops--;
                            if (borderCounty.troops == 0)
                            {
                                break;
                            }
                        }
                        int targetCheck = ra.Next(0, 100);
                        if (targetCheck > targetOdds)
                        {
                            weakestAdjacent.troops--;
                            if (weakestAdjacent.troops == 0)
                            {
                                break;
                            }
                        }
                    }
                    if (borderCounty.troops <= 0)
                    {
                        takeCounty(borderCounty, 1);
                        borderCounty.troops = weakestAdjacent.troops / 3;
                        weakestAdjacent.troops -= (weakestAdjacent.troops / 3);
                        if (checkWinner() == 1)
                        {
                            doVictory(1);
                            return;
                        }
                    }
                    else if (weakestAdjacent.troops <= 0)
                    {
                        takeCounty(weakestAdjacent, 2);
                        weakestAdjacent.troops = borderCounty.troops / 3;
                        borderCounty.troops -= (borderCounty.troops / 3);
                        Console.WriteLine("\tWon");
                        if (checkWinner() == 2)
                        {
                            doDefeat();
                            return;
                        }
                    } // END OF ATTACK
                }
            }
        } // Union AI for singleplayer

        public void updateGameLabels()
        {
            for (int z = 0; z < 21; z++)
            {
                troopLabels[z] = new TextWindow(new string[] { "\\5" + counties[z].troops }, Content,
                    new Vector2(counties[z].mapRectangle.Center.X, counties[z].mapRectangle.Center.Y) + labelOffsets[z], new Vector2(0,0), 0.1, 0);
                troopLabels[z].update(1);
                troopLabels[z].update(1);
                troopLabels[z].update(1);
                troopLabels[z].update(1);
            }
        } // update UI troop labels

        public void startWeek()
        {
            if (playerNumber == 1)
            {
                ++week;
                unionFunds += unionRevenue;
                funds += revenue;
                revWindow = new TextWindow(new string[] { playerNames[playerNumber] + "\\nWEEK " + week + "\\nFUNDS: " + funds + "\\nREVENUE: " + revenue },
                   Content, new Vector2(500, 100), new Vector2(225, 100), 1.0);
            }
            else
            {
                revWindow = new TextWindow(new string[] { playerNames[playerNumber] + "\\nWEEK " + week + "\\nFUNDS: " + unionFunds + "\\nREVENUE: " + unionRevenue },
                    Content, new Vector2(500, 100), new Vector2(225, 100), 1.0);
            }
            actionMenu = new Menu(
                new Vector2(500, 225),
                new string[] { "ATTACK", "MOVE TROOPS", "RECRUIT", "END WEEK" },
                new Menu.menuAction[] { selectAttackCounty, startTroopMove, startRecruit, endWeek },
                4, 1, "test-semifont.png", Content);
        } // week and turn handler

        public void endWeek()
        {
            revWindow = null;
            actionMenu = null;
            if (!multiPlayer)
            {
                doEnemyTurn();
            }
            else
            {
                int temp = playerNumber;
                playerNumber = enemyNumber;
                enemyNumber = temp;
            }
            updateGameLabels();
            startWeek();
        } // week and turn handler part 2

        public void startRecruit()
        {
            actionMenu = null;
            gpWindow = new TextWindow(new String[] { "SELECT A COUNTY" }, Content, new Vector2(25, 25), new Vector2(250, 25));
            selectingForRecruitment = true;
        }

        public int checkWinner()
        {
            bool christieWins = true;
            bool unionWins = true;
            for (int x = 0; x < 21; x++)
            {
                if (counties[x].occupied == 1)
                {
                    unionWins = false;
                }
                else if (counties[x].occupied == 2)
                {
                    christieWins = false;
                }
            }
            if (christieWins) return 1;
            else if (unionWins) return 2;
            else return 0;
        }

        public void attackCounty()
        {
            actionMenu = null;
            if (playerNumber == 1)
            {
                funds -= sourceCounty.troops / 2;
            }
            else
            {
                unionFunds -= sourceCounty.troops / 2;
            }
            if (targetCounty.occupied == 0)
            {
                takeCounty(targetCounty, playerNumber);
                ownedCounties++;
                Console.WriteLine(ownedCounties);
                if (checkWinner() == playerNumber)
                {
                    doVictory(playerNumber);
                    return;
                }
                cancelAttack();
                return;
            }
            Random ra = new Random();
            int sourceOdds = 50 * (sourceCounty.troops / targetCounty.troops);
            int targetOdds = 50 * (targetCounty.troops / sourceCounty.troops);
            int loss = 0;
            int gain = 0;
            for (int x = 0; x < 500; x++)
            {
                int sourceCheck = ra.Next(0, 100);
                if (sourceCheck > sourceOdds)
                {
                    sourceCounty.troops--;
                    loss++;
                    if (sourceCounty.troops == 0)
                    {
                        break;
                    }
                }
                int targetCheck = ra.Next(0, 100);
                if (targetCheck > targetOdds)
                {
                    targetCounty.troops--;
                    gain++;
                    if (targetCounty.troops == 0)
                    {
                        break;
                    }
                }
            }
            if (sourceCounty.troops <= 0)
            {
                takeCounty(sourceCounty, enemyNumber);
                sourceCounty.troops = targetCounty.troops / 3;
                targetCounty.troops -= (targetCounty.troops / 3);
                if (checkWinner() == enemyNumber)
                {
                    doVictory(enemyNumber);
                    return;
                }
            }
            else if (targetCounty.troops <= 0)
            {
                takeCounty(targetCounty, playerNumber);
                targetCounty.troops = sourceCounty.troops / 3;
                sourceCounty.troops -= (sourceCounty.troops / 3);
                Console.WriteLine(ownedCounties);
                if (checkWinner() == playerNumber)
                {
                    doVictory(playerNumber);
                    return;
                }
            }
            gpWindow = new TextWindow(new String[] { "KILLED: " + gain + " LOST: " + loss }, Content, new Vector2(25, 25), new Vector2(500, 25));
            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelAttack }, 1, 1, "test-semifont.png", Content);
        }

        public void cancelAttack()
        {
            selectingForAttack = 0;
            okayButton = null;
            gpWindow = null;
            okayButton = null;
            actionMenu = new Menu(
                new Vector2(500, 225),
                new string[] { "ATTACK", "MOVE TROOPS", "RECRUIT", "END WEEK" },
                new Menu.menuAction[] { selectAttackCounty, startTroopMove, startRecruit, endWeek },
                4, 1, "test-semifont.png", Content);
            int playerFunds = funds;
            if (playerNumber == 2) playerFunds = unionFunds;
            revWindow = new TextWindow(new string[] { playerNames[playerNumber] + "\\nWEEK " + week + "\\nFUNDS: " + playerFunds + "\\nREVENUE: " + revenue },
                Content, new Vector2(500, 100), new Vector2(225, 100),1.0);
            updateGameLabels();
        }

        public void cancelStartingCounty()
        {
            beginningMenu = null;
            beginningTextWindow = new TextWindow(new String[] { playerNames[beginningSelector] + " SELECT A COUNTY" },
                Content, new Vector2(25, 25), new Vector2(500, 25));
            return;
        }

        public void moveTroops()
        {
            if (playerNumber == 1)
            {
                funds -= 90;
            }
            else
            {
                Console.WriteLine("Subtracting union funds");
                unionFunds -= 90;
            }
            actionMenu = null;
            int troopsToMove = (int)(sourceCounty.troops / 3);
            sourceCounty.troops -= troopsToMove;
            targetCounty.troops += troopsToMove;
            gpWindow = new TextWindow(new String[] {
                "TROOPS IN " + sourceCounty.name.ToUpper() + ": " + sourceCounty.troops + "\\n" +
                "TROOPS IN " + targetCounty.name.ToUpper() + ": " + targetCounty.troops }, Content, new Vector2(25, 25), new Vector2(600, 50));
            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
            int playerFunds = funds;
            if (playerNumber == 2) playerFunds = unionFunds;
            revWindow = new TextWindow(new string[] { playerNames[playerNumber] + "\\nWEEK " + week + "\\nFUNDS: " + playerFunds + "\\nREVENUE: " + revenue },
                Content, new Vector2(500, 100), new Vector2(225, 100), 1.0);
        } // move troops from sourceCounty to targetCounty

        public void cancelTroopMove()
        {
            selectingForTroopTransfer = 0;
            gpWindow = null;
            okayButton = null;
            actionMenu = new Menu(
                new Vector2(500, 225),
                new string[] { "ATTACK", "MOVE TROOPS", "RECRUIT", "END WEEK" },
                new Menu.menuAction[] { selectAttackCounty, startTroopMove, startRecruit, endWeek },
                4, 1, "test-semifont.png", Content);
            updateGameLabels();
        }

        public void recruitTroops()
        {
            actionMenu = null;
            int nTroops = new Random().Next(30, 50);
            sourceCounty.troops += nTroops;
            if (playerNumber == 1)
            {
                funds -= 30;
            }
            else
            {
                unionFunds -= 30;
            }
            gpWindow = new TextWindow(new String[] {
                "RECRUITED " + nTroops + " TROOPS.\\n" + 
                "TROOPS IN " + sourceCounty.name.ToUpper() + ": " + sourceCounty.troops}, Content, new Vector2(25, 25), new Vector2(600, 50));
            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelRecruit }, 1, 1, "test-semifont.png", Content);
        }

        public void cancelRecruit()
        {
            selectingForRecruitment = false;
            okayButton = null;
            gpWindow = null;
            actionMenu = new Menu(
                new Vector2(500, 225),
                new string[] { "ATTACK", "MOVE TROOPS", "RECRUIT", "END WEEK" },
                new Menu.menuAction[] { selectAttackCounty, startTroopMove, startRecruit, endWeek },
                4, 1, "test-semifont.png", Content);
            int playerFunds = funds;
            if (playerNumber == 2) playerFunds = unionFunds;
            revWindow = new TextWindow(new string[] { playerNames[playerNumber] + "\\nWEEK " + week + "\\nFUNDS: " + playerFunds + "\\nREVENUE: " + revenue },
                Content, new Vector2(500, 100), new Vector2(225, 100), 1.0);
            updateGameLabels();
        }

        public void handleInput()
        {
            if (okayButton != null)
            {
                inputHandler.update(Keyboard.GetState());
                okayButton.handleInput(inputHandler);
                return;
            }
            if (actionMenu != null)
            {
                inputHandler.update(Keyboard.GetState());
                actionMenu.handleInput(inputHandler);
                if (selectingForAttack != 0 || selectingForRecruitment || selectingForTroopTransfer != 0)
                {
                    return;
                }
            }
            if (beginningMenu != null)
            {
                inputHandler.update(Keyboard.GetState());
                beginningMenu.handleInput(inputHandler);
                return;
            }
            MouseState currentMouseState = Mouse.GetState();
            inputHandler.updateMouse(currentMouseState);
            if (inputHandler.allowSingleClick())
            {
                List<County> selectedCounties = new List<County>();
                foreach (County c in counties)
                {
                    if (c.mapRectangle.Contains(currentMouseState.Position))
                    {
                        selectedCounties.Add(c);
                    }
                }
                if (selectedCounties.Count > 0)
                {
                    County closestCenter = selectedCounties[0];
                    for (int x = 1; x < selectedCounties.Count; x++)
                    {
                        if (Sprite.getRectangleDistance(selectedCounties[x].mapRectangle.Center, currentMouseState.Position) <
                            Sprite.getRectangleDistance(closestCenter.mapRectangle.Center, currentMouseState.Position))
                        {
                            closestCenter = selectedCounties[x];
                        }
                    }
                    individualCounty = closestCenter;
                    icTextWindow = new TextWindow(new string[] {
                        individualCounty.name.ToUpper() + " COUNTY\\n" +
                        "REVENUE: " + individualCounty.revenue + "\\n" +
                        "STATIONED TROOPS: " + individualCounty.troops + "\\n" +
                        "OWNER: " + individualCounty.getOwner() },
                        Content, new Vector2(350,630), new Vector2(400, 115));
                    if (beginning)
                    {
                        beginningTextWindow.advanceLine();
                        beginningTextWindow = new TextWindow(
                            new string[] { "SELECT " + individualCounty.name.ToUpper() },
                            Content, new Vector2(25, 25), new Vector2(300, 25));
                        beginningMenu = new Menu(
                            new Vector2(100, 100),
                            new string[] { "YES", "NO" },
                            new Menu.menuAction[] { selectStartingCounty, cancelStartingCounty },
                            2, 1, "test-semifont.png", Content);
                    }
                    else if (selectingForAttack == 1)
                    {
                        targetCounty = individualCounty;
                        selectingForAttack = 2;
                        gpWindow = new TextWindow(new string[] { "SELECT INVADING COUNTY" }, Content, new Vector2(25, 25), new Vector2(360, 25));
                    }
                    else if (selectingForAttack == 2)
                    {
                        sourceCounty = individualCounty;
                        bool adjacent = false;
                        for (int x = 0; x < sourceCounty.edges.Length; x++)
                        {
                            if (targetCounty == counties[sourceCounty.edges[x]])
                            {
                                adjacent = true;
                                break;
                            }
                        }
                        int playerFunds = funds;
                        if (playerNumber == 2) playerFunds = unionFunds;
                        if (sourceCounty.occupied != playerNumber)
                        {
                            gpWindow = new TextWindow(new String[] { "SOURCE COUNTY IS NOT YOURS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelAttack }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (targetCounty.occupied == playerNumber)
                        {
                            gpWindow = new TextWindow(new String[] { "TARGET COUNTY IS ALREADY YOURS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelAttack }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (!adjacent)
                        {
                            gpWindow = new TextWindow(new String[] { "TARGET COUNTY TOO FAR AWAY" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelAttack }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (playerFunds - (sourceCounty.troops / 2) < 0)
                        {
                            gpWindow = new TextWindow(new String[] { "NOT ENOUGH FUNDS  REQUIRED: " + (sourceCounty.troops / 2) }, Content, new Vector2(25, 25), new Vector2(650, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelAttack }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        selectingForAttack = 3;
                        int cost = sourceCounty.troops / 2;
                        gpWindow = new TextWindow(new string[] { "ATTACK " + targetCounty.name.ToUpper() + " FROM " + sourceCounty.name.ToUpper() + "\\nCOST: " + cost },
                            Content, new Vector2(25, 25), new Vector2(650, 50));
                        actionMenu = new Menu(
                            new Vector2(100, 100),
                            new string[] { "YES", "NO" },
                            new Menu.menuAction[] { attackCounty, cancelAttack },
                            2, 1, "test-semifont.png", Content);
                    }
                    else if (selectingForTroopTransfer == 1)
                    {
                        targetCounty = individualCounty;
                        selectingForTroopTransfer = 2;
                        gpWindow = new TextWindow(new string[] { "SELECT DONOR" }, Content, new Vector2(25, 25), new Vector2(360, 25));
                    }
                    else if (selectingForTroopTransfer == 2)
                    {
                        sourceCounty = individualCounty;
                        int playerFunds = funds;
                        if (playerNumber == 2) playerFunds = unionFunds;
                        bool connected = false;
                        List<County> visited = new List<County>();
                        Stack<County> DFSstack = new Stack<County>();
                        DFSstack.Push(sourceCounty);
                        while (DFSstack.Count != 0)
                        {
                            County chk = DFSstack.Pop();
                            if (chk == targetCounty && chk.occupied == playerNumber)
                            {
                                connected = true;
                                break;
                            }
                            else if (visited.Contains(chk))
                            {
                                continue;
                            }
                            for (int x = 0; x < chk.edges.Length; x++)
                            {
                                if (counties[chk.edges[x]].occupied == playerNumber) DFSstack.Push(counties[chk.edges[x]]);
                            }
                            visited.Add(chk);
                        }

                        if (sourceCounty.occupied != playerNumber)
                        {
                            gpWindow = new TextWindow(new String[] { "SOURCE COUNTY NOT YOURS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (targetCounty.occupied != playerNumber)
                        {
                            gpWindow = new TextWindow(new String[] { "RECIPIENT COUNTY NOT YOURS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (targetCounty == sourceCounty)
                        {
                            gpWindow = new TextWindow(new String[] { "THAT IS POINTLESS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (!connected)
                        {
                            gpWindow = new TextWindow(new String[] { "NO ROUTE FROM SOURCE TO RECIPIENT" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (playerFunds - 90 < 0)
                        {
                            gpWindow = new TextWindow(new String[] { "NOT ENOUGH FUNDS  REQUIRED: 90" }, Content, new Vector2(25, 25), new Vector2(650, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelTroopMove }, 1, 1, "test-semifont.png", Content);
                            return;
                        }

                        selectingForTroopTransfer = 3;
                        gpWindow = new TextWindow(new string[] { "MOVE " + (sourceCounty.troops/3) + " FROM " + 
                            sourceCounty.name.ToUpper() + " TO " + targetCounty.name.ToUpper() + "\\n" +
                            "COST: 90"},
                            Content, new Vector2(25, 25), new Vector2(650, 50));
                        actionMenu = new Menu(
                            new Vector2(100, 100),
                            new string[] { "YES", "NO" },
                            new Menu.menuAction[] { moveTroops, cancelTroopMove },
                            2, 1, "test-semifont.png", Content);
                    }
                    else if (selectingForRecruitment)
                    {
                        sourceCounty = individualCounty;
                        int playerFunds = funds;
                        if (playerNumber == 2) playerFunds = unionFunds;
                        if (sourceCounty.occupied != playerNumber)
                        {
                            gpWindow = new TextWindow(new String[] { "COUNTY NOT YOURS" }, Content, new Vector2(25, 25), new Vector2(500, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelRecruit }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        else if (playerFunds - 30 < 0)
                        {
                            gpWindow = new TextWindow(new String[] { "NOT ENOUGH FUNDS    REQUIRED: 30" }, Content, new Vector2(25, 25), new Vector2(650, 25));
                            okayButton = new Menu(new Vector2(700, 25), new string[] { "OK" }, new Menu.menuAction[] { cancelRecruit }, 1, 1, "test-semifont.png", Content);
                            return;
                        }
                        int cost = 30;
                        gpWindow = new TextWindow(new string[] { "RECRUIT TROOPS IN " + sourceCounty.name.ToUpper() + "\\nCOST: " + cost },
                            Content, new Vector2(25, 25), new Vector2(500, 50));
                        actionMenu = new Menu(
                            new Vector2(100, 100),
                            new string[] { "YES", "NO" },
                            new Menu.menuAction[] { recruitTroops, cancelRecruit },
                            2, 1, "test-semifont.png", Content);

                    }

                }

            }
        } // input handling and general game logic

        public void update(double dt)
        {

            if (icTextWindow != null)
            {
                icTextWindow.update(dt);
            }

            if (beginningTextWindow != null)
            {
                beginningTextWindow.update(dt);
            }
            if (gpWindow != null)
            {
                gpWindow.update(dt);
            }
            
            if (revWindow != null)
            {
                revWindow.update(dt);
            }

        } // update scrolling text
    }
}