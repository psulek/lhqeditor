import * as http from 'http';
import * as stringsContext from "./types/stringsContext";

(async () => {
    await stringsContext.initialize();

    const port = process.env.port || 1337;
    http.createServer(async function (req, res) {
        res.writeHead(200, { 'Content-Type': 'text/html' });

        const strings = await stringsContext.getStrings(req.url);

        let html = strings.messages.helloWorld + '<br\>';

        const languages = stringsContext.getLanguages();
        for (let i = 0; i < languages.length; i++) {
            const lang = languages[i];
            html += `<a href='/?lang=${lang}'>${lang}</a>&nbsp;`;
        }

        res.end(html);
    }).listen(port);

})();
