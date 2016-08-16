class Player
{
    static void Main(string[] args)
    {
        int bustersPerPlayer = int.Parse(Console.ReadLine()); // the amount of busters you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right

        Controleur controleur = new Controleur(myTeamId, bustersPerPlayer, ghostCount);
        //On initialise notre ï¿½quipe de buster
        for (int i = 0; i < bustersPerPlayer; i++)
        {
            if (myTeamId == 0)
            {
                controleur.ajouterBuster(i, new Buster(controleur.repere));
                controleur.ajouterEnnemi(i + bustersPerPlayer, new Buster(controleur.repereEnnemi));
            }
            else
            {
                controleur.ajouterBuster(i + bustersPerPlayer, new Buster(controleur.repere));
                controleur.ajouterEnnemi(i , new Buster(controleur.repereEnnemi));
            }
            
        }
        for (int i = 0; i < ghostCount; i++)
        {
            controleur.ajouterGhost(i, new Ghost());
        }
            // game loop
            while (true)
            {
                //On nettoie les fantome et ennemi que l'on voyait au tour précédent pour actualiser avec les nouveaux
                controleur.ghostVu.Clear();
                controleur.ennemiVu.Clear();
                int entities = int.Parse(Console.ReadLine()); // the number of busters and ghosts visible to you
                for (int i = 0; i < entities; i++)
                {
                    string[] inputs = Console.ReadLine().Split(' ');
                    int entityId = int.Parse(inputs[0]); // buster id or ghost id
                    int x = int.Parse(inputs[1]);
                    int y = int.Parse(inputs[2]); // position of this buster / ghost
                    int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
                    int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
                    int value = int.Parse(inputs[5]); // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.

                    if (entityType == -1)
                    {
                        controleur.updateGhost(entityId, new Point(x, y), value);
                        controleur.ghostVu.Add(entityId);
                    }
                    else
                    {
                        controleur.updateBuster(entityId, new Point(x, y), entityType, state, value);
                        if (entityType != myTeamId)
                            controleur.ennemiVu.Add(entityId);
                    }


                }
                foreach (var v in controleur.busters)
                {
                    string action = controleur.busterSimple(v.Key);
                    Console.WriteLine(action);
                }
            }
    }
}
