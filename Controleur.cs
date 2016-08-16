class Controleur {
    public int id { get; set; }
    public Point repere { get; set; }
    public Point repereEnnemi { get; set; }
    public Point basGauche { get; set; }
    public Point hautDroit { get; set; }
    public int width {get;set;}
    public int height {get;set;}
    public int bustersPerPlayer {get;set;}
    public int ghostCount {get;set;}
    public Dictionary<int,Buster> busters { get; set; }
    public Dictionary<int, Buster> ennemis { get; set; }
    public Dictionary<int,Ghost> ghosts { get; set; }

    public List<int> ghostVu { get; set; }
    public List<int> ennemiVu { get; set; }

    public const int VISION = 2200;
    public const int VISION_REPERE = 1555;
    public const int DIST_MIN_GHOST = 900;
    public const int DIST_MAX_GHOST = 1760;
    public const int DIST_MAX_STUN = 1760;
    public readonly Point[] KEYPOINT = {new Point(1555,1555), 
                                           new Point(1000,8000), 
                                           new Point(15000,8000), 
                                           new Point(8000,4500) };

    public Controleur(int id, int bpp, int gc)
    {
        this.id = id;
        if (id == 0) {
            repere = new Point(0, 0);
            repereEnnemi = new Point(16000, 9000);
        }
        else
        {
            repere = new Point(16000, 9000);
            repereEnnemi = new Point(0, 0);
        }
        this.basGauche = new Point(0, 9000);
        this.hautDroit = new Point(16000, 0);
        this.width = 16001;
        this.height = 9001;
        this.bustersPerPlayer = bpp;
        this.ghostCount = gc;
        this.busters = new Dictionary<int,Buster>(bustersPerPlayer);
        this.ghosts = new Dictionary<int,Ghost>(ghostCount);
        this.ennemis = new Dictionary<int,Buster>(bustersPerPlayer);
        this.ghostVu = new List<int>();
        this.ennemiVu = new List<int>();
    }

    public void ajouterBuster(int id, Buster b) {
        this.busters[id] = b;
    }

    public void ajouterGhost(int id, Ghost g)
    {
        this.ghosts[id] = g;
    }

    public void ajouterEnnemi(int id, Buster b)
    {
        this.ennemis[id] = b;
    }

    public void updateGhost(int id, Point position, int value)
    {
        this.ghosts[id].position = position;
        this.ghosts[id].nbChasseur = value;
    }

    public void updateBuster(int id, Point position, int type, int state, int value) {
        Buster b;
        if (type == this.id)
        {
            b = this.busters[id];
        } else {
            b= this.ennemis[id];
        }
        b.position = position;
        switch (state) {
            case 0 :
                b.hasFantome = false;
                break;
            case 1 : 
                b.hasFantome = true;
                b.idGhost = value;
                break;
            case 2 : 
                b.isStun = true;
                b.canMove = value;
                break;
        }
        if (!b.hasStun)
        {
            b.recupStun--;
            if (b.recupStun == 0)
                b.hasStun = true;
        }
        if (type == this.id)
            this.busters[id] = b;
        else 
            this.ennemis[id] = b;
    }


    /*Calcul la distance entre un ghost et un buster*/
    public int countDistance(Point g, Point b)
    {
        double dist = Math.Sqrt((g.X - b.X)*(g.X-b.X) + (g.Y - b.Y)*(g.Y-b.Y));
        return (int)dist;
    }


    /*
     * Cherche si le buster b voit un ghost
     * return en premier le ghost a portee de capture
     * sinon le ghost le plus pres
     * sinon -1
     * */
    public int hasFindGhost(Buster b)
    {  
        if (this.ghostVu.Count == 0)
            return -1;
        var list = new Dictionary<int, int>();
        foreach (int id in this.ghostVu)
        {
            int dist = countDistance(this.ghosts[id].position, b.position);
            if (dist <= VISION)
            {
                if (dist >= DIST_MIN_GHOST && dist <= DIST_MAX_GHOST)
                    return id;
                else
                    list[dist] = id;
            }              
        }
        if (list.Count == 0)
            return -1;
        else
        {
            return list[list.Keys.ToList().Min()];
        }
    }

    /*Cherche si un ennemi est proche du buster
     * Si oui, Retourne son id
     * Sinon retourne -1
     * */
    public int hasFindEnemy(Buster b)
    {
        if (this.ennemiVu.Count == 0)
            return -1;
        foreach (int id in this.ennemiVu)
        {
            int dist = countDistance(this.ennemis[id].position, b.position);
            if (dist <= DIST_MAX_STUN)
            {
                return id;
            }
        }
        return -1;
    }

    /*Renvoie un mouvement possible dans le cas ou le buster ne voit aucun fantome
     * */
    public string dontSeeGhost(Buster b)
    {
        var point = new List<Point>(KEYPOINT.ToList());
        //Si le buster n'a pas de direction prï¿½dï¿½fini
        if (b.direction.IsEmpty)
        {
           b.direction = point[new Random().Next(point.Count)];
        } //Sinon, si le buster est arrive a son objectif
        else if (b.position.Equals(b.direction)){
            int indice = point.IndexOf(b.direction);
            b.direction = point[(indice+1)%point.Count];
        }//Sinon, on est encore sur le chemin
        return "MOVE " + b.direction.X + " " + b.direction.Y + " Je me deplace vers " + b.direction.X + " " + b.direction.Y;
    }

    public string busterSimple(int idBuster)
    {
        Buster b  = this.busters[idBuster];
        string action = "";
        //Si le buster est a proximitï¿½ d'un ennemi
        int idEnnemy;
        idEnnemy = hasFindEnemy(b);
        if (idEnnemy != -1 && b.hasStun && !this.ennemis[idEnnemy].isStun)
        {
            action = "STUN " + idEnnemy + " Je stun " + idEnnemy;
            b.useStun();
        }
        else
        {
            //Si le buster a un fantome
            if (b.hasFantome)
            {
                int distWithRepere = countDistance(b.position, repere);
                //Si on est trop eloigne de la base
                if (distWithRepere > VISION_REPERE)
                {
                    if (id == 0)
                        action = "MOVE " + (repere.X + 1100) + " " + (repere.Y + 1100) + " Je me deplace vers " + repere.X + " " + repere.Y;
                    else
                        action = "MOVE " + (repere.X - 1100) + " " + (repere.Y - 1100) + " Je me deplace vers " + repere.X + " " + repere.Y;
                }
                //Sinon on est assez pres de la base pour relacher le fantome
                else
                {
                    action = "RELEASE" + " Je relache le ghost";
                    this.ghosts.Remove(b.idGhost);
                }
            }
            else
            //Sinon le buster n'a pas capture de fantome
            {
                int id = hasFindGhost(b);
                //Si le buster ne voit pas de fantome
                if (id == -1)
                {
                    action = dontSeeGhost(b);
                }
                //Sinon le buster voit un fantome 
                else
                {

                    int distWithGhost = countDistance(b.position, this.ghosts[id].position);
                    Console.Error.WriteLine("Le buster " + idBuster + "voit le fantome a " + distWithGhost);
                    //Si le buster est trop loin pour etre capturer, on se dirige vers lui
                    if (distWithGhost > DIST_MAX_GHOST)
                    {
                        action = "MOVE " + this.ghosts[id].position.X + " " + this.ghosts[id].position.Y;
                    }
                    //Sinon, si le buster est trop pres, on s'ï¿½loigne juste assez pour le capturer
                    else if (distWithGhost < DIST_MIN_GHOST)
                    {
                        Point temp = new Point();
                        if (b.position.X > 8000)
                            temp.X = b.position.X - 650;
                        else temp.X = b.position.X + 650;
                        if (b.position.Y > 4500)
                            temp.Y = b.position.Y - 650;
                        else temp.Y = b.position.Y + 650;
                        action = "MOVE " + temp.X + " " + temp.Y;
                    }
                    //Sinon on le capture
                    else
                    {
                        action = "BUST " + id;
                        b.hasFantome = true;
                    }
                }
            }
        }
       
        //On a traiter tout les cas, on retourne l'action
        return action;
    }
}
