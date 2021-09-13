"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
;
var NationGeography = /** @class */ (function () {
    function NationGeography(id, name, capital, latitude, longitude, imageUri, geographyXml) {
        if (id === void 0) { id = ""; }
        if (name === void 0) { name = ""; }
        if (capital === void 0) { capital = ""; }
        if (latitude === void 0) { latitude = 0; }
        if (longitude === void 0) { longitude = 0; }
        if (imageUri === void 0) { imageUri = ""; }
        if (geographyXml === void 0) { geographyXml = ""; }
        this.id = id;
        this.name = name;
        this.capital = capital;
        this.latitude = latitude;
        this.longitude = longitude;
        this.imageUri = imageUri;
        this.geographyXml = geographyXml;
    }
    NationGeography.fromData = function (data) {
        return new this(data.id, data.name, data.capital, data.latitude, data.longitude, data.imageUri, data.geographyXml);
    };
    return NationGeography;
}());
exports.NationGeography = NationGeography;
//# sourceMappingURL=NationGeography.js.map