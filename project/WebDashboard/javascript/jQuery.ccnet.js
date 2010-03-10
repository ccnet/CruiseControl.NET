/* jQuery goodness for CruiseControl.NET
 *
 * Requires jQuery 1.3.2
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
(function($) {
    // Add the display widget
    $.tablesorter.addWidget({
        id: 'statusDisplay',
        format: function(table) {
            $("tr.buildStatus", table.tBodies[0]).each(function() {
                var row = $(this);

                // Get the identifier of the owning row
                var id = row.attr('id');
                var linkedId = 'projectData' + id.substring(4);
                var parent = $('#' + linkedId);

                // Move the row to after the owning row
                row.insertAfter(parent);
            });
        }
    });

    // Helper function for initialising the project grid and all its fun stuff
    $.fn.initialiseProjectGrid = function(config) {
        // Initialise the configuration
        var defaultConfig = {
            widgets: ['statusDisplay'],
            sortList: [[0, 0]],
            textExtraction: function(node) {
                var t = $(node).text().trim();
                return t;
            }
        };
        config = $.extend(defaultConfig, config || {});

        // Initialise the sorting
        this.tablesorter(config);

        // Allow chaining
        return this;
    };
})(jQuery);