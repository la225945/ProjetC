using CourseVoiture;

/// Fonction principale main() implicitement déclarée
Course course = new();
try
{
    course.InitialiserJeu();
    while (course.MenuPrincipal())
    {
        if (!course.ChargerParcoursAuHasard())
        {
            course.AfficherMessageErreurParcours();
        }
        course.InitialiserPartie();
        while (course.PartieEnCours)
        {
            course.DebutComptageTemps();
            course.LireDeplacementAuClavier();
            course.AvancerRoute();
            course.DeplacerVoiture();
            course.DessinerParcours();
            course.FinComptageTemps();
            course.AttendreSelonLaVitesse();
            course.Accelerer();
        }
        course.DessinerParcours();
        course.DialogueDeFinDePartie();
    }
}
finally
{
    course.TerminerJeu();
}
