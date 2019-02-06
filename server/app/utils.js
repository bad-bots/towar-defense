function ResponseMessage(data, err) {
  return {
    error: ErrorMessage(err),
    data: data || ''
  };
}

function ErrorMessage(err) {
  return err
    ? {
        message: err.message || 'Something went wrong!',
        type: err.name || 'UnknownError'
      }
    : '';
}

module.exports = {
  ResponseMessage,
};
