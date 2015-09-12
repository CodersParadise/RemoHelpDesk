# RemoHelpDesk
Anwendung zur Fernwartung ein oder mehrerer Clients durch einen zentralen Server.

[Wiki](https://github.com/CodersParadise/RemoHelpDesk/wiki)


# Beschreibung
User beschweren sich oft über Probleme am Computer, welche oft durch wenige Mausklicks oder durch Änderungen von Konfigurationen behoben werden können oder wissen manchmal einfach nicht wie man ein Programm bedient und müssen dies erst demonstriert bekommen.
Deswegen wird in unserem Projekt ein Programm entstehen, welches eine Fernwartung von einem beliebigen PC im Netzwerk möglich macht.
Dies spart unnötige Laufwege von Mitarbeitern und Zeit und lässt damit ein Unternehmen effizienter und zielgerichteter arbeiten.

Das Projekt besteht aus einem Server und einem Client.
Beim Starten des Clients kann eine IP Adresse angegeben werden, alterantiv kann der Server mittels Broadcast ermittelt werden. Danach verbindet sich der Client mit dem Server.
Der Server besteht aus einem Fenster mit einer Liste von Clients und deren Eigenschaften. Die Clients können per Rechtsklick angesteuert und eine Fernwartungsfunktion aufgerufen werden (Bildschirmfoto, Maushilfe, etc.).
Außerdem kann ein Chatfenster für zusätzliche Kommunikation geöffnet werden.
Dem Mitarbeiter ist es somit möglich den Fehler schnell zu erfassen und erste Maßnahmen zur Behebung des Problems durchzuführen.

# ClientCore (Net 2.0)
Dynamic Link Library, has all the function to establish a connection to the server.
This .dll is compiled against Net 2.0 to allow it to be integrated into a far range of projects (Android/IOS Xamarin Projects, Old Clients, etc.)

# GuiClient (Net 4.0)
A Graphical User Interface wrapper for the ClientCore, this runs on all Net 4.0 supported machines.

# GuiServer (Net 4.0)
Server Interface to manage the client connections.

# NetworkObjects
A Shared Library between server and client, includes the Objects they can send over the wire.
