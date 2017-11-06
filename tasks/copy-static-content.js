var fs = require('fs-extra');

var filesToCopy = [
    'index.html',
    'silent-signin-callback.html',
    'tsconfig.json',
    'config/systemjs.config.js',
    'config/tsconfig-systemjs.json',
    'config/lite-server.config.json',
    'config/rollup-shared.config.js',
    'tasks/inline-assets.js'
];

filesToCopy.forEach(function (file) {

    try {
        var path = './node_modules/@cirrus/core.webui/' + file;
        if (fs.existsSync(path)) {
            fs.copySync(path, './' + file, { clobber: true });
            console.log('%s copied successfully!', file);
        }
    } catch (error) {
        console.error('%s could not be copied! %s', file, error);
    }

});