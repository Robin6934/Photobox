In dieser Datei wird erläutert, wie Visual Studio das Projekt erstellt hat.

Folgende Tools wurden zur Erstellung dieses Projekts verwendet:
- create-vite

Folgende Schritte wurden zur Erstellung dieses Projekts verwendet:
- Erstellen Sie ein VUE-Projekt mit create-vite: `npm init --yes vue@latest photobox.web.client -- --eslint  --typescript `.
- `vite.config.ts` wird mit dem Port aktualisiert.
- Fügen Sie `shims-vue.d.ts` für Basistypen hinzu.
- Projektdatei (`photobox.web.client.esproj`) erstellen.
- Erstellen Sie `launch.json`, um das Debuggen zu aktivieren.
- Projekt zur Projektmappe hinzufügen.
- Schreiben Sie diese Datei.
