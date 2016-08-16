class Buster {

    public int id { get; set; }
    public Point position {get;set;}
    public bool hasFantome {get;set;}
    public int idGhost {get;set;}
    public bool hasStun { get; set; }
    public int recupStun { get; set; }
    public bool isStun { get; set; }
    public int canMove { get; set; }


    public Point direction { get; set; }

    

    public Buster(Point position)
    {
        this.position = position;
        this.hasFantome = false;
        this.idGhost = -1;
        this.hasStun = true;
        this.recupStun = 20;
        this.isStun = false;
        this.canMove = 0;
        this.direction = new Point();
    }


    public void capturerGhost()
    {
        this.hasFantome = true;
    }

    public void relacherGhost()
    {
        this.hasFantome = false;
    }


    public void useStun()
    {
        this.hasStun = false;
        this.recupStun = 20;
    }
    

}
