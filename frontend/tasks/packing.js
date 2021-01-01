const fs = require('fs-extra');
const jsonfile = require('jsonfile');

const outputDir = './bundles';

const fileMapping = {
    '.npmrc': '.npmrc',
    'dist/@cirrus/module.catch-em-all.ui.d.ts': 'cirrus.module.catch-em-all.ui.d.ts',
};

/**
 * Writes the package.json
 */

var content = jsonfile.readFileSync('./package.json');

content.scripts = undefined;
content.devDependencies = undefined;

content.main = './cirrus.module.catch-em-all.ui.umd.js';
content.types = './cirrus.module.catch-em-all.ui.d.ts';

jsonfile.writeFileSync(outputDir + '/package.json', content, { spaces: 4 });

/**
 * Copies all required files
 */

Object.keys(fileMapping).forEach(function (source) {

    try {
        var dest = outputDir + '/' + fileMapping[source];
        fs.copySync(source, dest, { clobber: true });
        //console.log('%s copied successfully!', source);
    } catch (error) {
        console.error('%s could not be copied! %s', source, error);
    }

});