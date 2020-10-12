new Vue({
    el: "#form",
    data: {
        Email: '',
        Password: '',
        RememberMe: false,
        visible: false,
        visible_error:false,
        error_name: '',
        description:''
    },
    methods: {
        Authorize: function () {
            this.error_name = '';
            this.visible = true;
            this.visible_error = false;
            const request = {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "charset": "utf-8",
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: JSON.stringify({
                    Email: this.Email,
                    Password: this.Password,
                    RememberMe: this.RememberMe
                }),
                redirect:'follow'

            };

            fetch("/Account/Login", request)
                .then(response => {
                    if (response.status != 200) {
                        var x = response.json().then((data) => {
                            this.error_name = data.code
                            this.description = data.description
                        });
                        this.visible = false;
                        this.visible_error = true;
                    }
                    else {
                        location.href = "/Goods/Index";
                        this.visible = false;
                    }
                })
                .then(function (text) {                          
                    console.log('Request successful', text);
                    this.visible = false;
                })
                .catch(function (error) {                       
                    console.log('Request failed', error);
                    this.visible = false;
                });
        }
    }
})