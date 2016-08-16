class Ghost
{
    public Point position { get; set; }
    public int nbChasseur { get; set; }
    public bool isCaptured { get; set; }

    

    public Ghost()
    {
        this.position = new Point(int.MaxValue,int.MaxValue);
        this.nbChasseur = 0;
    }
}
