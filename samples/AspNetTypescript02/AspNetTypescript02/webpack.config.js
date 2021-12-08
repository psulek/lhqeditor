const path = require('path');
const webpack = require('webpack');
const UglifyJSPlugin = require('uglifyjs-webpack-plugin');

var folders = {
    Root: path.resolve(__dirname),
    NPM: path.resolve(__dirname, 'node_modules'),
    Output: path.resolve(__dirname, 'Scripts')
};

const package = require('./package.json');
const dateYear = new Date().getFullYear();
const packageVersion = package.version;
const headerBanner = `Company (c) ${dateYear}, ${package.description}, version: ${packageVersion}`;



module.exports = function (env) {
    const production = env && env.prod;
    const development = !production;
    const stats = env && env.stats === 'true';
    const mode = production ? 'production' : 'development';

    function consoleLog(log) {
        if (!stats) {
            console.log(log);
        }
    }

    consoleLog(` ${mode} Build `);

    const entry = path.resolve(__dirname, './Typescript/App.ts');
    const outputFileName = 'app' + (production ? '.min' : '') + '.js';

    const plugins = [
        new webpack.ProvidePlugin({
            jQuery: 'jQuery',
            $: 'jQuery',
            jquery: 'jQuery'
        }),

        new webpack.BannerPlugin({
            banner: headerBanner,
            raw: false,
            entryOnly: true
        }),

        // define DEBUG & PRODUCTION
        new webpack.DefinePlugin({
            DEBUG: !production,
            PRODUCTION: production
        })
    ];

    // minify JS for production build
    if (production) {
        plugins.push(new UglifyJSPlugin({
                sourceMap: true,
                uglifyOptions: {
                    ecma: 8,
                    keep_classnames: true,
                    keep_fnames: true,
                    mangle: {
                        keep_classnames: true,
                        keep_fnames: true
                    },
                    compress: {
                        warnings: false,
                        drop_debugger: true,
                        dead_code: true,
                        hoist_funs: false,
                        drop_console: true
                    },
                    output: {
                        beautify: false,
                        comments: /^!/
                    }
                }
            })
        );
    }

    var excludeRules = [
        /node_modules/
    ];

    var resolveAlias = {
    };

    // print out what will be excluded from build
    consoleLog(' Build excludes: ' + excludeRules.join(',') + ' ');

    // print out info about entry
    consoleLog(` Build entry: ${entry} `);

    consoleLog(' Aliases: ' + JSON.stringify(resolveAlias));

    // print out output filename
    consoleLog(` Output file: ` + path.join(folders.Output, outputFileName));

    const config = {
        entry: entry,

        mode: mode,

        target: 'web',

        devtool: 'source-map',

        output: {
            library: 'WebApp',
            libraryTarget: 'var',
            path: folders.Output,
            filename: outputFileName
        },

        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    exclude: /node_modules/,
                    loader: 'ts-loader'
                }
            ],

            // skip broken 'validate.js' amd-define! if does not work downgrade module 'request' to '2.65.0'
            noParse: function (content) {
                return /json-schema|asar/.test(content);
            }
        },

        resolve: {
            modules: [
                'node_modules',
                path.resolve(__dirname, 'src')
            ],

            extensions: ['.ts'], //, '.js'],

            alias: resolveAlias
        },

        externals: [
            {
                'jQuery': true
            }
        ],

        plugins: plugins,

        parallelism: 1,
        profile: development,
        bail: true
    };

    return config;
};