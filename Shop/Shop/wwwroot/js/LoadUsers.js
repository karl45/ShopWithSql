new Vue({
    el: "#form",
    data: {
        Email: '',
        EmailSearch: '',
        Password: '',
        RoleName: '',
        visible: false,
        errors: [] 
    },
    methods: {
        AddUser: function () {
            this.errors = [];
            error_list = this.errors
            this.visible = true;
            const request = {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: JSON.stringify({ Email: this.Email, Password: this.Password, RoleName: this.RoleName })
            };
            fetch("/Account/RegisterWithRole", request)
                .then(response => {
                    if (response.status != 200) {
                        var x = response.json().then(function (data) {
                            for (var index = 0; index < data.length; index++) {
                                var error_element = data[index];
                                error_list.push({ error: error_element.description, code: error_element.code })
                            }
                        });
                    
                        this.visible = false;
                    }
                    else {
                        var $table = $('#table')
                        var x = response.json().then((data) => {
                            $table.bootstrapTable('append', { email: this.Email, roleName: this.RoleName })
                        });
                        this.visible = false;
                    }
                })
                .then(function (text) {
                    //console.log('Request successful', text);
                    this.visible = false;

                })
                .catch(function (error) {
                    //console.log('Request failed', error);
                    this.visible = false;
                });
        },
        SearchUser: function () {
            const request = {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ Email: this.EmailSearch })
            };
            fetch("/Account/Search", request)
                .then(response => {
                        var $table = $('#table')
                        var x = response.json().then((data) => {
                            $table.bootstrapTable('load', data)
                        });
                    
                })
                .then(function (text) {
                    //console.log('Request successful', text);
                    this.visible = false;

                })
                .catch(function (error) {
                    //console.log('Request failed', error);
                    this.visible = false;
                });
        }
    }
})

    function ajaxRequest(parameter) {
        const request = {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        };
        fetch("/Account/GetUsers", request).then(response => {
                var x = response.json().then(function (data) {
                    parameter.success(data);
                });
            
        })
    }