// BEFORE: Original function with multiple issues
function processData(d) {
var r = [];
for(var i = 0; i < d.length; i++) {
if(d[i] != null) {
if(d[i].active == true) {
if(d[i].type == 'premium') {
var obj = {};
obj.id = d[i].id;
obj.name = d[i].firstName + ' ' + d[i].lastName;
obj.email = d[i].email;
obj.status = 'active';
r.push(obj);
}
}
}
}
return r;
}

// AFTER: Improved function with modern practices
function processData(users) {
  // Early return for invalid input
  if (!Array.isArray(users)) {
    return [];
  }

  return users
    .filter(user => user != null)
    .filter(user => user.active === true)
    .filter(user => user.type === 'premium')
    .map(user => ({
      id: user.id,
      name: `${user.firstName} ${user.lastName}`,
      email: user.email,
      status: 'active'
    }));
}

// Alternative version with even better error handling and validation
function processDataImproved(users) {
  // Input validation
  if (!Array.isArray(users)) {
    console.warn('processData: Expected array, received:', typeof users);
    return [];
  }

  return users
    .filter(user => {
      // Validate user object structure
      return user != null && 
             typeof user === 'object' &&
             user.active === true &&
             user.type === 'premium' &&
             user.id != null &&
             user.firstName != null &&
             user.lastName != null &&
             user.email != null;
    })
    .map(user => ({
      id: user.id,
      name: `${user.firstName} ${user.lastName}`.trim(),
      email: user.email,
      status: 'active'
    }));
}